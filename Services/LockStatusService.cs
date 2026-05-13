namespace SmartLockSystem.Services;

/// <summary>
/// Lưu trạng thái khóa real-time (Singleton - dùng chung toàn app)
/// </summary>
public class LockStatusService
{
    public bool IsLocked { get; private set; } = true;
    public string LastUser { get; private set; } = "";
    public DateTime LastChanged { get; private set; } = DateTime.UtcNow;

    public void SetUnlocked(string userName)
    {
        IsLocked = false;
        LastUser = userName;
        LastChanged = DateTime.Now;
        Console.WriteLine($"[STATUS] 🔓 UNLOCKED bởi {userName}");
    }

    public void SetLocked()
    {
        IsLocked = true;
        LastUser = "";
        LastChanged = DateTime.Now;
        Console.WriteLine($"[STATUS] 🔒 LOCKED");
    }
}
