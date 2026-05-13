using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLockSystem.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AccessCode", "FullName" },
                values: new object[] { "Jun", "Jun" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AccessCode", "AccessType", "FullName" },
                values: new object[] { "Phuong", "FaceID", "Phuong" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessCode", "AccessType", "CreatedAt", "FullName", "IsActive" },
                values: new object[] { 3, "Quan", "FaceID", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quan", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AccessCode", "FullName" },
                values: new object[] { "FACE_ID_BOSS", "Boss Nguyễn" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AccessCode", "AccessType", "FullName" },
                values: new object[] { "VIP_USER_QR_001", "QR", "Nhân viên VIP" });
        }
    }
}
