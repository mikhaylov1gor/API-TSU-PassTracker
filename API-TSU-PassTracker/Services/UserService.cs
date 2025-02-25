using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace API_TSU_PassTracker.Services
{
    public interface IUserService
    {
        Task register(UserRegisterModel userRegisterModel);
        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);
        Task logout(string token, ClaimsPrincipal user);
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

        public async Task register(UserRegisterModel newUser)
        {
            var isExists = await _context.User
                .AsNoTracking()
                .AnyAsync(u => u.Login == newUser.Login);

            var salt = _passwordHasher.GenerateSalt();
            var hashedPassword = _passwordHasher.GenerateHashPassword(newUser.Password, salt);

            if (isExists)
            {
                throw new ArgumentException("Пользователь уже существует.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUser.Name,
                Login = newUser.Login,
                PasswordHash = hashedPassword,
                Salt = salt,
                Roles = new List<Role>{ Role.Dean }, 
                Requests = new List<Request>()
            };

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

        }

        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var user = await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == loginCredentials.login);

            if (user == null)
            {
                throw new ArgumentException("Пользователь с таким логином не найден.");
            }

            var result = _passwordHasher.Verify(loginCredentials.password, user.PasswordHash, user.Salt);

            if (!result)
            {
                throw new UnauthorizedAccessException("Неверный пароль.");
            }

            return _jwtProvider.GenerateToken(user);
        }

        public async Task logout(string token, ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !Guid.TryParse(userId, out var parsedId))
            {
                throw new UnauthorizedAccessException("Невозможно определить идентификатор пользователя.");
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Токен не предоставлен или пуст.");
            }

            await _tokenBlackListService.AddTokenToBlackList(token);
        }
    }
}

