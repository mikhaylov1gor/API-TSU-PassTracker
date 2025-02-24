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
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel loginCredentials)
        {
            try
            {
                var token = await _userService.login(loginCredentials);
                return Ok(token);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message }); // 401
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка на сервере." }); // 500
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                token = token?.Replace("Bearer ", string.Empty);
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Токен не предоставлен или пуст." });
                }

                await _userService.logout(token, User);
                return Ok(new { message = "Выход выполнен успешно." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message }); // 401
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка при выходе из системы." }); // 500
            }
        }
    }
}
