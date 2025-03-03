using System;
using System.Security.Claims;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace API_TSU_PassTracker.Services;

public interface IRequestService
{
    Task CreateRequest(RequestModel request, ClaimsPrincipal user);
    Task<IEnumerable<RequestDTO>> GetAllRequests(ClaimsPrincipal user);
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

    public async Task CreateRequest(RequestModel request, ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var dbRequest = new Request
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            Confirmations = new List<Confirmation>() 
        };

        if (request.Confirmations != null && request.Confirmations.Count > 0)
        {
            foreach (var file in request.Confirmations)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                dbRequest.Confirmations.Add(new Confirmation
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FileData = ms.ToArray()
                });
            }
        }

        _context.Request.Add(dbRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RequestDTO>> GetAllRequests(ClaimsPrincipal user)
    {

        var requests = await _context.Request
            .Include(r => r.User) 
            .Include(r => r.Confirmations)
            .ToListAsync();

        var requestDtos = requests.Select(r => new RequestDTO
        {
            Id = r.Id,
            DateFrom = r.DateFrom,
            DateTo = r.DateTo,
            UserId = r.UserId,
            UserName = r.User.Name,
            Status = r.Status,

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

        existingRequest.DateTo = request.DateTo;

        _context.Request.Update(existingRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestDTO> GetRequestById(Guid id, ClaimsPrincipal user)
{
    var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
    var isDean = user.IsInRole("Dean");

    var request = await _context.Request
        .Include(r => r.User)
        .Include(r => r.Confirmations)
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
        DateFrom = request.DateFrom,
        DateTo = request.DateTo,
        Status = request.Status,
        UserId = request.UserId,
        UserName = request.User.Name
    };

    return requestDto;
}

}
