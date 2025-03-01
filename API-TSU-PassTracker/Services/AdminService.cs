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
<<<<<<< HEAD
        Task<bool> ChangeUserRole(UserRoleUpdateModel model);
=======
        Task<TokenResponseModel> register(UserRegisterModel user);
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
    }
    public class AdminService : IAdminService
    {
        private readonly TsuPassTrackerDBContext _context;
<<<<<<< HEAD
        

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
=======
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AdminService(
            TsuPassTrackerDBContext context,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<TokenResponseModel> register(UserRegisterModel user)
        {
            var isExists = await _context.User
                .AsNoTracking()
                .AnyAsync(u => u.Login == user.Login);

            var salt = _passwordHasher.GenerateSalt();
            var hashedPassword = _passwordHasher.GenerateHashPassword(user.Password, salt);

            if (isExists)
            {
                return null; // middleware
            }

            User newUser = new User
            {
                Name = user.Name,
                Roles = user.Roles,
                Login = user.Login,
                PasswordHash = hashedPassword,
                Salt = salt,
            };

            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return _jwtProvider.GenerateToken(newUser);
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
        }
    }
}
