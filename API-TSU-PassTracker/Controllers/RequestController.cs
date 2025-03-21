using System;
using System.Threading.Tasks;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_TSU_PassTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Student")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateRequest([FromForm] RequestModel request)
    {
        var user = HttpContext.User;
        var requestId = await _requestService.CreateRequest(request, user);
        return Ok(new { id = requestId });
    }

    [HttpGet("all")]
    [Authorize(Roles = "Dean, Teacher")]
    public async Task<ActionResult<ListLightRequestsDTO>> GetAllRequests(
        [FromQuery] ConfirmationType? confirmationType,
        [FromQuery] RequestStatus? status,
        [FromQuery] String? userName,
        [FromQuery] SortEnum? sort,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        var user = HttpContext.User;
        var requests = await _requestService.GetAllRequests(confirmationType, status, userName, user, sort, page, size);
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
    [Authorize(Roles = "Student, Dean")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateRequest(Guid id, [FromForm] RequestUpdateModel request)
    {
        var user = HttpContext.User;
        await _requestService.UpdateRequest(id, request, user);
        return Ok(new { message = "Заявка обновлена успешно." });
    }
}