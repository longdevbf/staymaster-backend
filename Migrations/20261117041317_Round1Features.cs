using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.API.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Đợt 1: đánh giá có trả lời / ẩn, thông báo trong app (thêm cột duong_dan), lý do từ chối phòng,
    /// giá cuối tuần, ngày chặn phòng, cấu hình hệ thống. Bảng thong_bao và yeu_thich đã có sẵn nên chỉ ánh xạ.
    /// </summary>
    public partial class Round1Features : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "gia_cuoi_tuan",
                table: "phong",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ly_do_tu_choi",
                table: "phong",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "bi_an",
                table: "danh_gia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ly_do_an",
                table: "danh_gia",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngay_phan_hoi",
                table: "danh_gia",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phan_hoi_chu_nha",
                table: "danh_gia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cau_hinh_he_thong",
                columns: table => new
                {
                    khoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    gia_tri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ngay_cap_nhat = table.Column<DateTime>(type: "datetime", nullable: false),
                    nguoi_cap_nhat_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cau_hinh_he_thong", x => x.khoa);
                });

            migrationBuilder.CreateTable(
                name: "ngay_chan_phong",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phong_id = table.Column<int>(type: "int", nullable: false),
                    tu_ngay = table.Column<DateTime>(type: "date", nullable: false),
                    den_ngay = table.Column<DateTime>(type: "date", nullable: false),
                    ly_do = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ngay_chan_phong", x => x.id);
                    table.ForeignKey(
                        name: "FK_ngay_chan_phong_phong_phong_id",
                        column: x => x.phong_id,
                        principalTable: "phong",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // thong_bao và yeu_thich đã có sẵn trong DB (QL_NhaNghi.sql): chỉ thêm cột đường dẫn
            migrationBuilder.AddColumn<string>(
                name: "duong_dan",
                table: "thong_bao",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ngay_chan_phong_phong_id_tu_ngay",
                table: "ngay_chan_phong",
                columns: new[] { "phong_id", "tu_ngay" });

            migrationBuilder.CreateIndex(
                name: "ix_thong_bao_nguoi_dung_da_doc",
                table: "thong_bao",
                columns: new[] { "nguoi_dung_id", "da_doc" });


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cau_hinh_he_thong");

            migrationBuilder.DropTable(
                name: "ngay_chan_phong");

            migrationBuilder.DropIndex(
                name: "ix_thong_bao_nguoi_dung_da_doc",
                table: "thong_bao");

            migrationBuilder.DropColumn(
                name: "duong_dan",
                table: "thong_bao");

            migrationBuilder.DropColumn(
                name: "gia_cuoi_tuan",
                table: "phong");

            migrationBuilder.DropColumn(
                name: "ly_do_tu_choi",
                table: "phong");

            migrationBuilder.DropColumn(
                name: "bi_an",
                table: "danh_gia");

            migrationBuilder.DropColumn(
                name: "ly_do_an",
                table: "danh_gia");

            migrationBuilder.DropColumn(
                name: "ngay_phan_hoi",
                table: "danh_gia");

            migrationBuilder.DropColumn(
                name: "phan_hoi_chu_nha",
                table: "danh_gia");
        }
    }
}
