using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace API_TSU_PassTracker.Services
{
    public interface IUserService
    {
        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);
        Task<ActionResult> logout(string token, ClaimsPrincipal user);
    }
    public class UserService : IUserService
    {
        private readonly TsuPassTrackerDBContext _context;
        private readonly ITokenBlackListService _tokenBlackListService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public UserService(
            TsuPassTrackerDBContext context,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            ITokenBlackListService tokenBlackListService)
        {

            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _tokenBlackListService = tokenBlackListService;
        }

        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var user = await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == loginCredentials.login);

            if (user == null)
            {
                return null; //400 validation
            }

            var result = _passwordHasher.Verify(loginCredentials.password, user.PasswordHash, user.Salt);

            if (!result)
            {
                return null; //400 validation
            }
            else
            {
                return _jwtProvider.GenerateToken(user);
            }
        }

        public async Task<ActionResult> logout(string token, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !Guid.TryParse(userId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); // ex
            }

            if (!string.IsNullOrEmpty(token))
            {
                await _tokenBlackListService.AddTokenToBlackList(token);
                return null;
            }
            else
            {
                throw new UnauthorizedAccessException(); // ex
            }
        }
    }
}

