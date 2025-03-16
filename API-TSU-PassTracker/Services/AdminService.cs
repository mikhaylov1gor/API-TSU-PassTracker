using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_TSU_PassTracker.Services
{
    public interface IAdminService
    {
        Task<bool> ChangeUserRole(UserRoleUpdateModel model);
        Task<bool> confirmAccount(Guid userId, bool status);
        Task<bool> confirmRequest(Guid requestId, RequestStatus status);
        Task<ActionResult<UserPagedListModel>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles, string group, int page, int size);
        Task<byte[]> downloadRequests(DateTime? dateFrom, DateTime? dateTo);
    }
    public class AdminService : IAdminService
    {
        private readonly TsuPassTrackerDBContext _context;

        public AdminService(
            TsuPassTrackerDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangeUserRole(UserRoleUpdateModel userRoleUpdateModel)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == userRoleUpdateModel.Id);

            if (user == null)
            {
                throw new ArgumentException("Пользователь не найден");
            }

            user.Roles = userRoleUpdateModel.Roles;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> confirmAccount(Guid userId, bool status)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false; 
            }

            user.IsConfirmed = status;
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<bool> confirmRequest(Guid requestId, RequestStatus status)
        {
            var request = await _context.Request
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return false;
            }

            request.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ActionResult<UserPagedListModel>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles, string group, int page, int size)
        {
            var query = _context.User.AsQueryable();

            if (page < 1 || size < 1)
            {
                throw new ArgumentException("Номер страницы и размер не могут быть меньше 1");
            }

            if (onlyConfirmed)
            {
                query = query.Where(u => u.IsConfirmed);   
            }

            if (onlyTheseRoles != null && onlyTheseRoles.Any())
            {
                query = query.Where(u => u.Roles.Any(ur => onlyTheseRoles.Contains(ur)));
            }

            if (group != null) 
            {
                query = query.Where(u => u.Group.Contains(group));
            }

            var totalItems = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    IsConfirmed = u.IsConfirmed,
                    Name = u.Name,
                    Group = u.Group,
                    Roles = u.Roles,
                })
                .ToListAsync();

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };

            return new UserPagedListModel
            {
                users = users,
                pagination = pageInfo
            };
        }

        public async Task<byte[]> downloadRequests(DateTime? dateFrom, DateTime? dateTo)
        {
            var requestsQuery = _context.Request
            .Include(r => r.User)
            .Where(r => r.Status == RequestStatus.Approved)
            .AsQueryable();

            if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value > dateTo.Value)
            {
                throw new ArgumentException("dateFrom не может быть позже чем dateTo.");
            }

            if (dateFrom.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.CreatedDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var dateToEndOfDay = dateTo.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                requestsQuery = requestsQuery.Where(r => r.CreatedDate <= dateToEndOfDay);
            }

            var requests = await requestsQuery.ToListAsync();



            if (!requests.Any())
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Заявки");
                    ws.Cell("A1").Value = "Нет подтвержденных заявок";
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Заявки");

                ws.Cell("A1").Value = "Пользователь";
                ws.Cell("B1").Value = "Группа";
                ws.Cell("C1").Value = "Дата начала";
                ws.Cell("D1").Value = "Дата окончания";
                ws.Cell("E1").Value = "Причина";

                int row = 2;
                foreach (var req in requests)
                {
                    ws.Cell(row, 1).Value = $"{req.User.Name}";
                    ws.Cell(row, 2).Value = $"{req.User.Group}";
                    ws.Cell(row, 3).Value = $"{req.DateFrom:dd.MM.yyyy}";
                    ws.Cell(row, 4).Value = $"{req.DateTo:dd.MM.yyyy}";
                    ws.Cell(row, 5).Value = $"{(req.ConfirmationType == ConfirmationType.Family ? "Семейная" :
                        req.ConfirmationType == ConfirmationType.Educational ? "Учебная" : "Медицинская")}";

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
