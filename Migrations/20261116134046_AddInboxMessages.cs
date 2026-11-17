using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ngay_tin_nhan_cuoi",
                table: "hop_thu_cau_hoi",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "hop_thu_tin_nhan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cau_hoi_id = table.Column<int>(type: "int", nullable: false),
                    nguoi_gui_id = table.Column<int>(type: "int", nullable: true),
                    ben_gui = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hop_thu_tin_nhan", x => x.id);
                    table.ForeignKey(
                        name: "FK_hop_thu_tin_nhan_hop_thu_cau_hoi_cau_hoi_id",
                        column: x => x.cau_hoi_id,
                        principalTable: "hop_thu_cau_hoi",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hop_thu_tin_nhan_cau_hoi_id_ngay_tao",
                table: "hop_thu_tin_nhan",
                columns: new[] { "cau_hoi_id", "ngay_tao" });

            // Chuyển câu hỏi / câu trả lời cũ thành tin nhắn của cuộc trò chuyện
            migrationBuilder.Sql(@"
INSERT INTO hop_thu_tin_nhan (cau_hoi_id, nguoi_gui_id, ben_gui, noi_dung, ngay_tao)
SELECT id, khach_hang_id, N'customer', cau_hoi, ngay_tao FROM hop_thu_cau_hoi;

INSERT INTO hop_thu_tin_nhan (cau_hoi_id, nguoi_gui_id, ben_gui, noi_dung, ngay_tao)
SELECT id, nguoi_tra_loi_id, CASE WHEN doi_tac_id IS NULL THEN N'support' ELSE N'host' END, tra_loi, COALESCE(ngay_tra_loi, ngay_tao)
FROM hop_thu_cau_hoi WHERE tra_loi IS NOT NULL;

UPDATE hop_thu_cau_hoi SET ngay_tin_nhan_cuoi = COALESCE(ngay_tra_loi, ngay_tao);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hop_thu_tin_nhan");

            migrationBuilder.DropColumn(
                name: "ngay_tin_nhan_cuoi",
                table: "hop_thu_cau_hoi");
        }
    }
}
