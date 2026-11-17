using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.API.Migrations
{
    /// <summary>
    /// Thêm bảng ma_dat_lai_mat_khau (quên mật khẩu).
    /// Migration này cũng đồng bộ snapshot với DB thật: bảng tien_ich, phong_tien_ich và các cột
    /// doi_tac.so_du_vi, co_so_luu_tru.url_anh_dai_dien đã có sẵn trong DB nên không tạo lại ở đây.
    /// </summary>
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ma_dat_lai_mat_khau",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nguoi_dung_id = table.Column<int>(type: "int", nullable: false),
                    ma_bam = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    het_han = table.Column<DateTime>(type: "datetime2", nullable: false),
                    da_dung_luc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ma_dat_lai_mat_khau", x => x.id);
                    table.ForeignKey(
                        name: "FK_ma_dat_lai_mat_khau_nguoi_dung_nguoi_dung_id",
                        column: x => x.nguoi_dung_id,
                        principalTable: "nguoi_dung",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ma_dat_lai_mat_khau_ma_bam",
                table: "ma_dat_lai_mat_khau",
                column: "ma_bam",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ma_dat_lai_mat_khau_nguoi_dung_id",
                table: "ma_dat_lai_mat_khau",
                column: "nguoi_dung_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ma_dat_lai_mat_khau");
        }
    }
}
