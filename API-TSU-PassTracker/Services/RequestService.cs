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
            DateTo = request.DateTo
        };

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
            UserName = r.User?.Name, 
            Status = r.Status,

        });

        return requestDtos;
    }

    

}
