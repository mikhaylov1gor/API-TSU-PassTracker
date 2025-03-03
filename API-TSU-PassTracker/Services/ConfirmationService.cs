// API_TSU_PassTracker.Services/ConfirmationService.cs
using System.Security.Claims;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace API_TSU_PassTracker.Services
{
    public interface IConfirmationService
    {
        Task CreateConfirmation(Guid requestId, ConfirmationModel confirmation, ClaimsPrincipal user);
    }

    public class ConfirmationService : IConfirmationService
    {
        private readonly TsuPassTrackerDBContext _context;

        public ConfirmationService(TsuPassTrackerDBContext context)
        {
            _context = context;
        }

        public async Task CreateConfirmation(Guid requestId, ConfirmationModel confirmation, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var request = await _context.Request
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new KeyNotFoundException("Запрос не найден.");

            if (request.UserId != userId)
                throw new UnauthorizedAccessException("Вы не можете добавить подтверждение к этому запросу.");

            switch (confirmation.ConfirmationType)
            {
                case ConfirmationType.Medical:
                case ConfirmationType.Educational:
                    if (confirmation.Files == null || confirmation.Files.Count == 0)
                        throw new ArgumentException("Для данного типа подтверждения требуется минимум один файл.");
                    break;
                case ConfirmationType.Family:
                    break;
                default:
                    throw new ArgumentException("Неизвестный тип подтверждения.");
            }

            var dbConfirmation = new Confirmation
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                ConfirmationType = confirmation.ConfirmationType
            };

            if (confirmation.Files != null && confirmation.Files.Count > 0)
            {
                foreach (var file in confirmation.Files)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    dbConfirmation.Files.Add(new ConfirmationFile
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        FileData = ms.ToArray()
                    });
                }
            }

            request.Confirmation = dbConfirmation;
            _context.Confirmation.Add(dbConfirmation);
            await _context.SaveChangesAsync();
        }

        
    }
}