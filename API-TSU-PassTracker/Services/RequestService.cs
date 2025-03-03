using System;
using System.Security.Claims;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Models.DB;
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
        {
            throw new ArgumentException("DateTo должна быть позже чем DateFrom.");
        }

        var dbRequest = new Request
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateFrom = request.DateFrom.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(request.DateFrom, DateTimeKind.Utc) 
                : request.DateFrom.ToUniversalTime(),
            DateTo = request.DateTo.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(request.DateTo, DateTimeKind.Utc) 
                : request.DateTo.ToUniversalTime(),
            CreatedDate = DateTime.UtcNow
        };

        _context.Request.Add(dbRequest);
        await _context.SaveChangesAsync();

        return dbRequest.Id;
    }

    public async Task<IEnumerable<LightRequestDTO>> GetAllRequests(ClaimsPrincipal user)
    {
        var requests = await _context.Request
            .Include(r => r.User)
            .Include(r => r.Confirmation)
            .ThenInclude(c => c.Files)
            .ToListAsync();

        var requestDtos = requests.Select(r => new LightRequestDTO
        {
            Id = r.Id,
            CreatedDate = r.CreatedDate,
            DateFrom = r.DateFrom,
            DateTo = r.DateTo,
            UserName = r.User.Name,
            Status = r.Status,
            ConfirmationType = r.Confirmation != null
            ? r.Confirmation.ConfirmationType
            : null,
        });

        return requestDtos;
    }

    public async Task UpdateRequest(Guid id, RequestUpdateModel request, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

        var existingRequest = await _context.Request
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRequest == null)
        {
            throw new KeyNotFoundException("Запрос не найден.");
        }

        if (existingRequest.UserId != userId)
        {
            throw new UnauthorizedAccessException("Вы не можете изменить этот запрос.");
        }

        existingRequest.DateTo = request.DateTo.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.DateTo, DateTimeKind.Utc) 
            : request.DateTo.ToUniversalTime();

        _context.Request.Update(existingRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestDTO> GetRequestById(Guid id, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isDean = user.IsInRole("Dean");

        var request = await _context.Request
            .Include(r => r.User)
            .Include(r => r.Confirmation)
            .ThenInclude(c => c.Files)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            throw new KeyNotFoundException("Запрос не найден.");
        }

        if (!isDean && request.UserId != userId)
        {
            throw new UnauthorizedAccessException("Вы не можете просматривать этот запрос.");
        }

        var requestDto = new RequestDTO
        {
            Id = request.Id,
            CreatedDate = request.CreatedDate,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            Status = request.Status,
            UserId = request.UserId,
            UserName = request.User.Name,
            Confirmation = request.Confirmation != null 
                ? new ConfirmationDTO
                    {
                        Id = request.Confirmation.Id,
                        ConfirmationType = request.Confirmation.ConfirmationType,
                        Files = request.Confirmation.Files.Select(f => new ConfirmationFileDTO
                        {
                            Id = f.Id,
                            FileName = f.FileName,
                            FileBase64 = Convert.ToBase64String(f.FileData)
                        }).ToList()
                    }
                : null
        };

        return requestDto;
    }
}