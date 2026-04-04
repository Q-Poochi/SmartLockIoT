using Microsoft.AspNetCore.Mvc;
using SmartLockSystem.Services;
using SmartLockSystem.Models;

namespace SmartLockSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartLockController : ControllerBase
    {
        private readonly ISmartLockService _smartLockService;

        public SmartLockController(ISmartLockService smartLockService)
        {
            _smartLockService = smartLockService;
        }
        
        [HttpPost("Command")]
        public async Task<IActionResult> SendCommand([FromBody] LockCommandRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceId))
            {
                return BadRequest("Miss Information");
            }

            var isSuccess = await _smartLockService.SendLockCommandAsync(request);
            if (isSuccess)
            {
                return Ok(new {Message= "Command sent successfully", status=true });
            }
            return StatusCode (500, "Error sending command");
        }
    }
}
