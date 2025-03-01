using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Http;
<<<<<<< HEAD
using Microsoft.AspNetCore.Http.HttpResults;
=======
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace API_TSU_PassTracker.Services
{
    public interface IUserService
    {
<<<<<<< HEAD
        Task register(UserRegisterModel userRegisterModel);
        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);
        Task logout(string token, ClaimsPrincipal user);
=======
        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);
        Task<ActionResult> logout(string token, ClaimsPrincipal user);
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
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

<<<<<<< HEAD
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

=======
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var user = await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == loginCredentials.login);

            if (user == null)
            {
<<<<<<< HEAD
                throw new ArgumentException("Пользователь с таким логином не найден.");
=======
                return null; //400 validation
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
            }

            var result = _passwordHasher.Verify(loginCredentials.password, user.PasswordHash, user.Salt);

            if (!result)
            {
<<<<<<< HEAD
                throw new UnauthorizedAccessException("Неверный пароль.");
            }

            return _jwtProvider.GenerateToken(user);
        }

        public async Task logout(string token, ClaimsPrincipal user)
=======
                return null; //400 validation
            }
            else
            {
                return _jwtProvider.GenerateToken(user);
            }
        }

        public async Task<ActionResult> logout(string token, ClaimsPrincipal user)
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !Guid.TryParse(userId, out var parsedId))
            {
<<<<<<< HEAD
                throw new UnauthorizedAccessException("Невозможно определить идентификатор пользователя.");
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Токен не предоставлен или пуст.");
            }

            await _tokenBlackListService.AddTokenToBlackList(token);
=======
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
>>>>>>> a423c2c6bc36eb14a97aa7030955dbc3fe588e0a
        }
    }
}

