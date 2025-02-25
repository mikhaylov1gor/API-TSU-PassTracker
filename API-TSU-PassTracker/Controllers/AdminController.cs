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
    }
}
