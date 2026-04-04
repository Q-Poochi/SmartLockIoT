using SmartLockSystem.Models;
namespace SmartLockSystem.Services
{
    public class SmartLockService : ISmartLockService
    {
        public async Task<bool> SendLockCommandAsync(LockCommandRequest request)
        {
            string action = request.Unlock ? "Mở khóa" : "Khóa";
            Console.WriteLine($"[SmartLockService] Đang thực hiện lệnh: {action} cho thiết bị {request.DeviceId}");

            await Task.Delay(500);
            Console.WriteLine($"[SmartLockService] Lệnh {action} đã được gửi thành công đến thiết bị {request.DeviceId}");
            return true;

        }
    }
}
