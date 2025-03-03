using System;
using System.Threading.Tasks;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_TSU_PassTracker.Controllers
{
    [Route("api/confirmation")]
    [ApiController]
    public class ConfirmationController : ControllerBase
    {
        private readonly IConfirmationService _confirmationService;

        public ConfirmationController(IConfirmationService confirmationService)
        {
            _confirmationService = confirmationService;
        }

        [HttpPost("{requestId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateConfirmation(Guid requestId, [FromForm] ConfirmationType confirmationType, [FromForm] List<IFormFile>? files = null)
        {
            var confirmation = new ConfirmationModel
            {
                ConfirmationType = confirmationType,
                Files = files
            };
            await _confirmationService.CreateConfirmation(requestId, confirmation, User);
            return Ok(new { message = "Подтверждающий документ успешно добавлен" });
        }


    }
}
