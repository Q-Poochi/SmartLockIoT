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

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAndUnlock([FromBody] VerifyRequest request)
        {
            // Giả lập Database (Tạm thời thầy để List, bài sau mình nối SQL)
            var validCodes = new List<string> { "VIP_USER_QR_001", "FACE_ID_BOSS" };

            if (validCodes.Contains(request.AccessCode))
            {
                // 1. Nhận diện đúng người -> C# tự động gọi MQTT bắn lệnh MỞ CỬA xuống ESP32
                var unlockReq = new LockCommandRequest { DeviceId = "esp32/led/control", Unlock = true };
                await _smartLockService.SendLockCommandAsync(unlockReq);

                // 2. (Gợi ý thêm): Lưu lịch sử "request.AccessCode vừa vào lúc 10h" vào Database...

                return Ok(new { message = "Xác nhận thành công! Đã mở cửa.", user = request.AccessCode });
            }
            else
            {
                // Sai người, nháy đèn đỏ báo động hoặc bỏ qua
                return BadRequest(new { message = "Khuôn mặt hoặc QR không có trong hệ thống!" });
            }
        }

    }
}
