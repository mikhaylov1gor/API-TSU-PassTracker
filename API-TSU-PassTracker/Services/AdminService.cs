using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace API_TSU_PassTracker.Services
{
    public interface IAdminService
    {
        Task<bool> ChangeUserRole(UserRoleUpdateModel model);
        Task<bool> confirmAccount(Guid userId, bool status);
        Task<bool> confirmRequest(Guid requestId, RequestStatus status);
        Task<ActionResult<List<UserModel>>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles);
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

        public async Task<ActionResult<List<UserModel>>> getUsers(bool onlyConfirmed, List<Role> onlyTheseRoles)
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
    }
}
