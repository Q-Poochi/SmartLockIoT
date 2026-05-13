using System.ComponentModel.DataAnnotations;

namespace SmartLockSystem.Models;

/// <summary>
/// Bảng Người Dùng - Lưu danh sách người được phép ra vào
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AccessCode { get; set; } = string.Empty;

    [MaxLength(20)]
    public string AccessType { get; set; } = "QR"; // "QR" hoặc "FaceID"

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
