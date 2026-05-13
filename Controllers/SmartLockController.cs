using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data;
using SmartLockSystem.Services;
using SmartLockSystem.Models;

namespace SmartLockSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartLockController : ControllerBase
    {
        private readonly ISmartLockService _smartLockService;
        private readonly SmartLockDbContext _db;

        public SmartLockController(ISmartLockService smartLockService, SmartLockDbContext db)
        {
            _smartLockService = smartLockService;
            _db = db;
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
            // Truy vấn Database thật thay vì List giả lập!
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.AccessCode == request.AccessCode && u.IsActive);

            if (user != null)
            {
                // 1. Nhận diện đúng người -> Bắn MQTT mở cửa
                var unlockReq = new LockCommandRequest { DeviceId = "test_door", Unlock = true };
                await _smartLockService.SendLockCommandAsync(unlockReq);

                // 2. GHI VÀO SỔ LỊCH SỬ (AccessLog) - "Ai vào lúc mấy giờ"
                _db.AccessLogs.Add(new AccessLog
                {
                    AccessCode = request.AccessCode,
                    AccessType = request.Type,
                    IsSuccess = true,
                    Message = $"{user.FullName} đã mở cửa thành công",
                    UserId = user.Id,
                    DeviceId = 1,
                    Timestamp = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();

                return Ok(new { message = "Xác nhận thành công! Đã mở cửa.", user = user.FullName });
            }
            else
            {
                // Ghi lại lần xâm nhập trái phép
                _db.AccessLogs.Add(new AccessLog
                {
                    AccessCode = request.AccessCode,
                    AccessType = request.Type,
                    IsSuccess = false,
                    Message = "Truy cập bị từ chối - Mã không hợp lệ",
                    Timestamp = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();

                return BadRequest(new { message = "Khuôn mặt hoặc QR không có trong hệ thống!" });
            }
        }

        // API mới: Xem lịch sử ra vào (cho giao diện Web hiển thị)
        [HttpGet("access-logs")]
        public async Task<IActionResult> GetAccessLogs()
        {
            var logs = await _db.AccessLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(50)
                .Select(l => new
                {
                    l.Id,
                    l.AccessCode,
                    l.AccessType,
                    l.IsSuccess,
                    l.Message,
                    l.Timestamp
                })
                .ToListAsync();

            return Ok(logs);
        }

        // API mới: Xem danh sách người dùng được phép
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Where(u => u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.AccessType,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}

