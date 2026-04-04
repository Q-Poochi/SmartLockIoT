using SmartLockSystem.Models;
namespace SmartLockSystem.Services
{
    public interface ISmartLockService
    {
        Task<bool> SendLockCommandAsync(LockCommandRequest request);
    }
}
