using Microsoft.EntityFrameworkCore;

namespace SmartLockSystem.Data;

public class SmartLockDbContext : DbContext
{
    public SmartLockDbContext(DbContextOptions<SmartLockDbContext> options) : base(options)
    {
    }

    // Bảng Người Dùng - Lưu danh sách người được phép vào nhà
    public DbSet<Models.User> Users { get; set; }

    // Bảng Lịch Sử Ra Vào - Ghi lại ai mở cửa lúc mấy giờ
    public DbSet<Models.AccessLog> AccessLogs { get; set; }

    // Bảng Thiết Bị - Quản lý danh sách khóa cửa ESP32
    public DbSet<Models.Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data: Users thật từ hệ thống Python Face Recognition
        modelBuilder.Entity<Models.User>().HasData(
            new Models.User
            {
                Id = 1,
                FullName = "Jun",
                AccessCode = "Jun",
                AccessType = "FaceID",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Models.User
            {
                Id = 2,
                FullName = "Phuong",
                AccessCode = "Phuong",
                AccessType = "FaceID",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Models.User
            {
                Id = 3,
                FullName = "Quan",
                AccessCode = "Quan",
                AccessType = "FaceID",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );

        // Seed data mẫu: 1 thiết bị ESP32 mặc định
        modelBuilder.Entity<Models.Device>().HasData(
            new Models.Device
            {
                Id = 1,
                DeviceName = "Khóa Cửa Chính",
                MqttTopic = "esp32/led",
                Location = "Cửa trước",
                IsOnline = false,
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );
    }
}
