using System;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_TSU_PassTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestController : ControllerBase
{
    private IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateRequest([FromBody] RequestModel request)
    {
        var user = HttpContext.User;
        await _requestService.CreateRequest(request, user);
        return Ok(new { message = "Заявка создана успешно." });
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllRequests()
    {
        var user = HttpContext.User;
        var requests = await _requestService.GetAllRequests(user);
        return Ok(requests);
    }

}
