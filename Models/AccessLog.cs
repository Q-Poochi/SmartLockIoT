using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLockSystem.Models;

/// <summary>
/// Bảng Lịch Sử Ra Vào - Ghi lại ai mở cửa lúc mấy giờ
/// </summary>
public class AccessLog
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string AccessCode { get; set; } = string.Empty;

    [MaxLength(20)]
    public string AccessType { get; set; } = string.Empty; // "QR", "FaceID", "Manual"

    public bool IsSuccess { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Khóa ngoại: Liên kết tới thiết bị nào đã mở
    public int? DeviceId { get; set; }

    [ForeignKey("DeviceId")]
    public Device? Device { get; set; }

    // Khóa ngoại: Liên kết tới người dùng nào đã vào
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
