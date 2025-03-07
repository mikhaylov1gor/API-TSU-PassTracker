using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
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
        Task<ActionResult<List<UserModel>>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles, string group);
        Task<byte[]> downloadRequests(List<RequestStatus> status);
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

        public async Task<ActionResult<List<UserModel>>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles, string group)
        {
            var query = _context.User.AsQueryable();

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
                query = query.Where(u => u.Group == group);
            }

            return await query
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    IsConfirmed = u.IsConfirmed,
                    Name = u.Name,
                    Group = u.Group,
                    Roles = u.Roles,
                })
                .ToListAsync();
        }

        public async Task<byte[]> downloadRequests(List<RequestStatus> status)
        {
            var requests = await _context.Request
                .Where(r => status.Contains(r.Status))
                .Include(r => r.User)
                .ToListAsync();

            if (!requests.Any())
            {
                return Encoding.UTF8.GetBytes("Нет заявок с таким статусом.");
            }

            var content = string.Join("\n---\n", requests.Select(r =>
                $"({string.Join(", ", r.User.Roles.Select(role => role.ToString()))})\n" +
                $"ID заявки: {r.Id}\n" +
                $"Пользователь: {r.User.Name} ({(r.User.IsConfirmed ? "Аккаунт подтвержден" : "Аккаунт не подтвержден")})\n" +
                $"Группа: {r.User.Group}\n" +
                $"Пропустил занятия с {r.DateFrom:dd.MM.yyyy} по {r.DateTo:dd.MM.yyyy}\n" +
                $"Причина: {(r.ConfirmationType == ConfirmationType.Family? "Семейная" : 
                            r.ConfirmationType == ConfirmationType.Medical ? "Медицинская" : "Учебная")}\n" +
                $"Статус заявки: {(r.Status == RequestStatus.Approved ? "Подтверждена" :
                            r.Status == RequestStatus.Rejected ? "Отклонена" : "В обработке")}"
            ));

            return Encoding.UTF8.GetBytes(content);
        }

    }
}
