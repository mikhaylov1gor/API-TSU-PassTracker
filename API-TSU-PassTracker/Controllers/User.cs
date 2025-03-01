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
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseModel>> login(LoginCredentialsModel loginCredentials)
        {
            var response = await _userService.login(loginCredentials);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _userService.logout(token, User);
            return Ok(response);
        }
    }
}
