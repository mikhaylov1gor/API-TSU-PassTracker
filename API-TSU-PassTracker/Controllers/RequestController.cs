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
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateRequest([FromBody] RequestModel request)
    {
        var user = HttpContext.User;
        await _requestService.CreateRequest(request, user);
        return Ok(new { message = "Заявка создана успешно." });
    }

    [HttpGet("all")]
    [Authorize(Roles = "Dean")]
    public async Task<IActionResult> GetAllRequests()
    {
        var user = HttpContext.User;
        var requests = await _requestService.GetAllRequests(user);
        return Ok(requests);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAllMyRequests()
    {
        var user = HttpContext.User;
        var requests = await _requestService.GetAllMyRequests(user);
        return Ok(requests);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Student, Dean")]
    public async Task<IActionResult> GetRequestById(Guid id)
    {
        var user = HttpContext.User;
        var request = await _requestService.GetRequestById(id, user);
        return Ok(request);
    }

    [HttpPut("update/{id}")] 
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateRequest(Guid id, [FromBody] RequestUpdateModel request)
    {
        var user = HttpContext.User;
        await _requestService.UpdateRequest(id, request, user); 
        return Ok(new { message = "Заявка обновлена успешно." });
    }

}
