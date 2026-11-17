using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.API.Migrations
{
    /// <summary>
    /// Ánh xạ bảng log_he_thong (đã có sẵn trong DB) vào model để ghi nhật ký thao tác.
    /// Không thay đổi DB; chỉ cập nhật snapshot để các migration sau không tạo lại bảng này.
    /// </summary>
    public partial class MapAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
