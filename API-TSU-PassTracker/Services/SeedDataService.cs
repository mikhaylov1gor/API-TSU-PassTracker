using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_TSU_PassTracker.Services
{
    public interface ISeedDataService
    {
        Task SeedUsers();
    }

    public class SeedDataService : ISeedDataService
    {
        private readonly TsuPassTrackerDBContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public SeedDataService(TsuPassTrackerDBContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedUsers()
        {
            if (!await _context.User.AnyAsync())
            {
                var json = File.ReadAllText("Users.json");

                var tempUsers = JsonSerializer.Deserialize<List<TempUser>>(json);

                if (tempUsers != null)
                {
                    var users = new List<User>();

                    foreach (var tempUser in tempUsers)
                    {
                        var salt = _passwordHasher.GenerateSalt();
                        var hashedPassword = _passwordHasher.GenerateHashPassword(tempUser.Login, salt);

                        var user = new User
                        {
                            Id = Guid.NewGuid(),
                            IsConfirmed = tempUser.IsConfirmed,
                            Name = tempUser.Name,
                            Group = tempUser.Group,
                            Login = tempUser.Login,
                            PasswordHash = hashedPassword,
                            Salt = salt,
                            Roles = tempUser.Roles
                                .Select(roleString => Enum.Parse<Role>(roleString))
                                .ToList(),
                        };

                        if (tempUser.Requests != null)
                        {
                            user.Requests = tempUser.Requests
                                .Select(requestSeed => new Request
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedDate = DateTime.SpecifyKind(requestSeed.CreatedDate, DateTimeKind.Utc),
                                    DateFrom = DateTime.SpecifyKind(requestSeed.DateFrom, DateTimeKind.Utc),
                                    DateTo = requestSeed.DateTo != null
                                        ? DateTime.SpecifyKind(requestSeed.DateTo.Value, DateTimeKind.Utc)
                                        : (DateTime?)null,
                                    Status = Enum.Parse<RequestStatus>(requestSeed.Status),
                                    ConfirmationType = Enum.Parse<ConfirmationType>(requestSeed.ConfirmationType),
                                    UserId = user.Id
                                })
                                .ToList();
                        }

                        users.Add(user);
                    }

                    await _context.User.AddRangeAsync(users);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public class TempUser
        {
            public bool IsConfirmed { get; set; }
            public string Name { get; set; }
            public string Group { get; set; }
            public List<string> Roles { get; set; }
            public string Login { get; set; }
            public List<RequestSeedModel> Requests { get; set; } 
        }

        public class RequestSeedModel
        {
            public DateTime CreatedDate { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
            public string Status { get; set; }
            public string ConfirmationType { get; set; }
        }
    }
}