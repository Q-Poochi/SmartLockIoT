using System.ComponentModel.DataAnnotations;

namespace SmartLockSystem.Models;

/// <summary>
/// Bảng Thiết Bị - Quản lý danh sách khóa cửa ESP32
/// </summary>
public class Device
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string MqttTopic { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public bool IsOnline { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
