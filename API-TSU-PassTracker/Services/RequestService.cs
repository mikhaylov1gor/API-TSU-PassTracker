using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace API_TSU_PassTracker.Services;

public interface IRequestService
{
    Task<Guid> CreateRequest(RequestModel request, ClaimsPrincipal user);
    Task<IEnumerable<LightRequestDTO>> GetAllRequests(ClaimsPrincipal user);
    Task<RequestDTO> GetRequestById(Guid id, ClaimsPrincipal user);
    Task UpdateRequest(Guid id, RequestUpdateModel request, ClaimsPrincipal user);
}

public class RequestService : IRequestService
{
    private readonly TsuPassTrackerDBContext _context;

    public RequestService(TsuPassTrackerDBContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateRequest(RequestModel request, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

        if (request.DateTo <= request.DateFrom)
            throw new ArgumentException("DateTo должна быть позже чем DateFrom.");

        ValidateConfirmation(request.ConfirmationType, request.Files);

        var dbRequest = new Request
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateFrom = EnsureUtc(request.DateFrom),
            DateTo = EnsureUtc(request.DateTo),
            CreatedDate = DateTime.UtcNow,
            ConfirmationType = request.ConfirmationType,
            Files = new List<RequestFile>()
        };

        if (request.Files != null && request.Files.Count > 0)
        {
            foreach (var file in request.Files)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                dbRequest.Files.Add(new RequestFile
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FileData = ms.ToArray()
                });
            }
        }

        _context.Request.Add(dbRequest);
        await _context.SaveChangesAsync();

        return dbRequest.Id;
    }

    public async Task<IEnumerable<LightRequestDTO>> GetAllRequests(ClaimsPrincipal user)
    {
        var requests = await _context.Request
            .Include(r => r.User)
            .Include(r => r.Files)
            .ToListAsync();

        var requestDtos = requests.Select(r => new LightRequestDTO
        {
            Id = r.Id,
            CreatedDate = r.CreatedDate,
            DateFrom = r.DateFrom,
            DateTo = r.DateTo,
            UserName = r.User.Name,
            Status = r.Status,
            ConfirmationType = r.ConfirmationType
        });

        return requestDtos;
    }

    public async Task<RequestDTO> GetRequestById(Guid id, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isDean = user.IsInRole("Dean");

        var request = await _context.Request
            .Include(r => r.User)
            .Include(r => r.Files)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            throw new KeyNotFoundException("Запрос не найден.");

        if (!isDean && request.UserId != userId)
            throw new UnauthorizedAccessException("Вы не можете просматривать этот запрос.");

        return new RequestDTO
        {
            Id = request.Id,
            CreatedDate = request.CreatedDate,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            Status = request.Status,
            UserId = request.UserId,
            UserName = request.User.Name,
            ConfirmationType = request.ConfirmationType,
            Files = request.Files.Select(f => new RequestFileDTO
            {
                Id = f.Id,
                FileName = f.FileName,
                FileBase64 = Convert.ToBase64String(f.FileData)
            }).ToList()
        };
    }

    public async Task UpdateRequest(Guid id, RequestUpdateModel request, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

        var existingRequest = await _context.Request
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRequest == null)
            throw new KeyNotFoundException("Запрос не найден.");

        if (existingRequest.UserId != userId)
            throw new UnauthorizedAccessException("Вы не можете изменить этот запрос.");

        existingRequest.DateTo = EnsureUtc(request.DateTo);

        _context.Request.Update(existingRequest);
        await _context.SaveChangesAsync();
    }

    private DateTime EnsureUtc(DateTime date) =>
        date.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
            : date.ToUniversalTime();

    private void ValidateConfirmation(ConfirmationType type, List<IFormFile>? files)
    {
        switch (type)
        {
            case ConfirmationType.Medical:
            case ConfirmationType.Educational:
                if (files == null || files.Count == 0)
                    throw new ArgumentException("Для данного типа подтверждения требуется минимум один файл.");
                break;
            case ConfirmationType.Family:
                break;
            default:
                throw new ArgumentException("Неизвестный тип подтверждения.");
        }
    }
}