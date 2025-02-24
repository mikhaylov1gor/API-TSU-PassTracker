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
        Task<TokenResponseModel> register(UserRegisterModel user);
    }
    public class AdminService : IAdminService
    {
        private readonly TsuPassTrackerDBContext _context;
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
        }
    }
}
