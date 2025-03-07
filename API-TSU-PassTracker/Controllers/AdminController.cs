using API_TSU_PassTracker.Filters;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace API_TSU_PassTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private IAdminService _adminService;

        public AdminController(IAdminService _service)
        {
            _adminService = _service;
        }

        [HttpPut("role")]
        [Authorize(Roles = "Dean")]
        public async Task<ActionResult> ChangeUserRole([FromBody] UserRoleUpdateModel userRoleUpdateModel)
        {
            if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();
                    return StatusCode(StatusCodes.Status400BadRequest, 
                        new ErrorResponse(400, "Ошибка валидации", errors));
                }

                var result = await _adminService.ChangeUserRole(userRoleUpdateModel);
                return Ok(new { message = "Роль пользователя успешно обновлена" });

        }

        [HttpPut("confirm-account")]
        [Authorize(Roles = "Dean")]
        public async Task<IActionResult> ConfirmAccount(Guid userId, bool isConfirmed)
        {
            var response = await _adminService.confirmAccount(userId, isConfirmed);
            return response ? Ok("Статус аккаунта изменен") : NotFound("Пользователь не найден.");
        }

        [HttpPut("confirm-request")]
        [Authorize(Roles = "Dean")]
        public async Task<IActionResult> ConfirmRequest(Guid requestId, RequestStatus status)
        {
            var response = await _adminService.confirmRequest(requestId, status);
            return response ? Ok("Статус заявки изменен") : NotFound("Заявка не найдена.");
        }

        [HttpGet("users")]
        [Authorize(Roles = "Dean")]
        public async Task<ActionResult<List<UserModel>>> getAllUsers(
            [FromQuery] bool onlyConfirmed, 
            [FromQuery] List<Role> onlyTheseRoles,
            [FromQuery] string? group)
        {
            var response = await _adminService.getUsers(onlyConfirmed, onlyTheseRoles, group);
            return Ok(response);
        }

        [HttpGet("download-requests")]
        [Authorize(Roles = "Teacher, Dean")]
        public async Task<IActionResult> downloadRequests(
            [FromQuery] List<RequestStatus> status)
        {
            var response = await _adminService.downloadRequests(status);
            return File(response, "text/plain", "requests.txt");
        }
    }
}
