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
        Task<TokenResponseModel> register(UserRegisterModel newUser);
        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);
        Task logout(string token, ClaimsPrincipal user);
        Task<UserModel> getProfile(ClaimsPrincipal userClaims);
        Task<LightRequestsPagedListModel> GetAllMyRequests(ConfirmationType? confirmationType, RequestStatus? status, SortEnum? sort, ClaimsPrincipal user, int page, int size);
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

        public async Task<TokenResponseModel> register(UserRegisterModel newUser)
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

            if (newUser.Roles.Contains(Role.Student) && newUser.Group == null)
            {
                throw new ArgumentException("Для пользователя с ролью студент группа обязательна");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                IsConfirmed = false,
                Name = newUser.Name,
                Login = newUser.Login,
                PasswordHash = hashedPassword,
                Salt = salt,
                Roles = newUser.Roles,
                Requests = new List<Request>(),
                Group = newUser.Group
            };

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            return _jwtProvider.GenerateToken(user);
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

        public async Task<UserModel> getProfile (ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !Guid.TryParse (userId, out var parsedId))
            {
                throw new UnauthorizedAccessException("Невозможно определить идентификатор пользователя.");
            }

            var user = await _context.User
                .AsNoTracking()
                .Where(u => u.Id == parsedId)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    IsConfirmed = u.IsConfirmed,
                    Name = u.Name,
                    Group = u.Group,
                    Roles = u.Roles,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            return user;
        }

        public async Task<LightRequestsPagedListModel> GetAllMyRequests(ConfirmationType? confirmationType, RequestStatus? status, SortEnum? sort, ClaimsPrincipal user, int page, int size)
        {
            var requests =  _context.Request.AsQueryable();

            if (page < 1 || size < 1)
            {
                throw new ArgumentException("Номер страницы и размер не могут быть меньше 1");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                requests = requests.Where(r => r.User.Id == userId);
            }
            else
            {
                throw new Exception("Невозможно идентифицировать пользователя");
            }
           

           if(status.HasValue) {
            requests = status switch
            {
                RequestStatus.Pending => requests.Where(r => r.Status == RequestStatus.Pending),
                RequestStatus.Approved => requests.Where(r => r.Status == RequestStatus.Approved),
                RequestStatus.Rejected => requests.Where(r => r.Status == RequestStatus.Rejected),
                _ => throw new ArgumentException("Неизвестный статус запроса."),
            };
           }

            if(sort.HasValue) {
                requests = sort switch
            {
                SortEnum.CreatedAsc => requests.OrderBy(r => r.CreatedDate),
                SortEnum.CreatedDesc => requests.OrderByDescending(r => r.CreatedDate),
                _ => throw new ArgumentException("Неизвестный тип сортировки."),
            };
            }

            if(confirmationType.HasValue) {
                requests = confirmationType switch
            {
                ConfirmationType.Educational => requests.Where(r => r.ConfirmationType == ConfirmationType.Educational),
                ConfirmationType.Medical => requests.Where(r => r.ConfirmationType == ConfirmationType.Medical),
                ConfirmationType.Family => requests.Where(r => r.ConfirmationType == ConfirmationType.Family),
                _ => throw new ArgumentException("Неизвестный тип подтвреждающих документов."),
            };
            }

            var totalItems = await requests.CountAsync();

            var lightRequestsDtos = await requests
                .Skip((page - 1) * size)
                .Take(size)
                .Select(r => new LightRequestDTO
                {
                    Id = r.Id,
                    CreatedDate = r.CreatedDate,
                    DateFrom = r.DateFrom,
                    DateTo = r.DateTo,
                    Status = r.Status,
                    UserName = r.User.Name,
                    ConfirmationType = r.ConfirmationType,
                }).ToListAsync();

            var listLightRequestsDTO = new ListLightRequestsDTO
            {
                ListLightRequests = lightRequestsDtos
            };

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };

            return new LightRequestsPagedListModel
            {
                requests = listLightRequestsDTO,
                pagination = pageInfo,
            };
        }
    }
}

