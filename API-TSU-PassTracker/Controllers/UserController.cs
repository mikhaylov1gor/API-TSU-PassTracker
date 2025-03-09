using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_TSU_PassTracker.Models;
using API_TSU_PassTracker.Models.DTO;
using Microsoft.AspNetCore.Authorization;
namespace API_TSU_PassTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService _service)
        {
            _userService = _service;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> register(UserRegisterModel newUser)
        {
            var response = await _userService.register(newUser);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel loginCredentials)
        {
            var token = await _userService.login(loginCredentials);
            return Ok(token);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
        {
            token = token?.Replace("Bearer ", string.Empty);
                if (string.IsNullOrEmpty(token))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();
                    return StatusCode(StatusCodes.Status400BadRequest, 
                        new ErrorResponse(400, "Ошибка валидации", errors));
                }

                await _userService.logout(token, User);
                return Ok(new { message = "Выход выполнен успешно." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetProfile()
        {
            var response = await _userService.getProfile(User);
            return Ok(response);
        }

        [HttpGet("requests")]
        [Authorize(Roles = "Student, Dean")]
        public async Task<ActionResult<ListLightRequestsDTO>> GetAllMyRequests()
        {
            var user = HttpContext.User;
            var requests = await _userService.GetAllMyRequests(user);
            return Ok(requests);
        }
    }
}
