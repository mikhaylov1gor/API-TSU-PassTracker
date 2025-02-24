using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
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

        [HttpPost]
        public async Task<ActionResult> register(UserRegisterModel newUser)
        {
            try
            {
                var response = await _adminService.register(newUser);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка на сервере." }); // 500
            }
        }
    }
}
