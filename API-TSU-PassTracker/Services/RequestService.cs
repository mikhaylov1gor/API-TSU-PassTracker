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
    Task<ListLightRequestsDTO> GetAllRequests(ClaimsPrincipal user);
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
        ValidateDatesAndFiles(request);

        var dbRequest = new Request
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateFrom = EnsureUtc(request.DateFrom),
            DateTo = request.DateTo.HasValue ? EnsureUtc(request.DateTo.Value) : 
                     request.ConfirmationType == ConfirmationType.Medical ? EnsureUtc(request.DateFrom.AddDays(7)) : null,
            CreatedDate = DateTime.UtcNow,
            ConfirmationType = request.ConfirmationType
        };

        if (request.Files != null && request.Files.Count > 0)
        {
            dbRequest.Files = new byte[request.Files.Count][];
            for (int i = 0; i < request.Files.Count; i++)
            {
                using var ms = new MemoryStream();
                await request.Files[i].CopyToAsync(ms);
                dbRequest.Files[i] = ms.ToArray();
            }
        }

        _context.Request.Add(dbRequest);
        await _context.SaveChangesAsync();
        return dbRequest.Id;
    }

    public async Task<ListLightRequestsDTO> GetAllRequests(ClaimsPrincipal user)
    {
        var requests = await _context.Request
            .Include(r => r.User)
            .ToListAsync();

        var lightRequestsDtos = requests.Select(r => new LightRequestDTO
            {
                Id = r.Id,
                CreatedDate = r.CreatedDate,
                DateFrom = r.DateFrom,
                DateTo = r.DateTo,
                Status = r.Status,
                ConfirmationType = r.ConfirmationType,
            }).ToList();

        var listLightRequestsDTO = new ListLightRequestsDTO
        {
            ListLightRequests = lightRequestsDtos
        };

        return listLightRequestsDTO;
    }

    public async Task<RequestDTO> GetRequestById(Guid id, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isDean = user.IsInRole("Dean");

        var request = await _context.Request
            .Include(r => r.User)
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
            Files = request.Files?.Select(f => Convert.ToBase64String(f)).ToList() ?? new List<string>()
        };
    }

    public async Task UpdateRequest(Guid id, RequestUpdateModel request, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isDean = user.IsInRole("Dean");

        var existingRequest = await _context.Request
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRequest == null)
            throw new KeyNotFoundException("Запрос не найден.");

        if (!isDean && existingRequest.UserId != userId)
            throw new UnauthorizedAccessException("Вы не можете изменить этот запрос.");

        if (!isDean && existingRequest.ConfirmationType == ConfirmationType.Educational && 
            (request.DateFrom.HasValue || request.DateTo.HasValue))
            throw new UnauthorizedAccessException("Студент не может изменять даты учебной заявки.");

        if (request.DateFrom.HasValue)
            existingRequest.DateFrom = EnsureUtc(request.DateFrom.Value);

        if (request.DateTo.HasValue)
        {
            if (request.DateTo.Value <= existingRequest.DateFrom)
                throw new ArgumentException("DateTo должна быть позже чем DateFrom.");
            existingRequest.DateTo = EnsureUtc(request.DateTo.Value);
        }
        else if (existingRequest.ConfirmationType == ConfirmationType.Medical && existingRequest.DateTo == null)
        {
            existingRequest.DateTo = EnsureUtc(existingRequest.DateFrom.AddDays(7));
        }

        if (request.Files != null) 
        {
            if (request.Files.Count > 0)
            {
                existingRequest.Files = new byte[request.Files.Count][];
                for (int i = 0; i < request.Files.Count; i++)
                {
                    using var ms = new MemoryStream();
                    await request.Files[i].CopyToAsync(ms);
                    existingRequest.Files[i] = ms.ToArray();
                }
            }
            else
            {
                existingRequest.Files = Array.Empty<byte[]>();
            }
        }

        // update request status
        existingRequest.Status = RequestStatus.Pending;

        ValidateDatesAndFiles(new RequestModel
        {
            DateFrom = existingRequest.DateFrom,
            DateTo = existingRequest.DateTo,
            ConfirmationType = existingRequest.ConfirmationType,
            Files = request.Files ?? (existingRequest.Files?.Length > 0 ? new List<IFormFile>() : null),
        });

        await _context.SaveChangesAsync();
    }

    private DateTime EnsureUtc(DateTime date) =>
        date.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : date.ToUniversalTime();

    private void ValidateDatesAndFiles(RequestModel request)
    {
        switch (request.ConfirmationType)
        {
            case ConfirmationType.Medical:
                if (request.Files == null || request.Files.Count == 0)
                    throw new ArgumentException("Для больничного требуется минимум один файл.");
                break;
            case ConfirmationType.Educational:
                if (!request.DateTo.HasValue)
                    throw new ArgumentException("Для учебной заявки DateTo обязательна.");
                if (request.DateTo.Value <= request.DateFrom)
                    throw new ArgumentException("DateTo должна быть позже чем DateFrom.");
                if (request.Files == null || request.Files.Count == 0)
                    throw new ArgumentException("Для учебной заявки требуется минимум один файл.");
                break;
            case ConfirmationType.Family:
                if (!request.DateTo.HasValue)
                    throw new ArgumentException("Для семейной заявки DateTo обязательна.");
                if (request.DateTo.Value <= request.DateFrom)
                    throw new ArgumentException("DateTo должна быть позже чем DateFrom.");
                break;
            default:
                throw new ArgumentException("Неизвестный тип подтверждения.");
        }
    }
}