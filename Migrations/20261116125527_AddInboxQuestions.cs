using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInboxQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hop_thu_cau_hoi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    khach_hang_id = table.Column<int>(type: "int", nullable: false),
                    doi_tac_id = table.Column<int>(type: "int", nullable: true),
                    phong_id = table.Column<int>(type: "int", nullable: true),
                    cau_hoi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    tra_loi = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    nguoi_tra_loi_id = table.Column<int>(type: "int", nullable: true),
                    nguon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ngay_tra_loi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hop_thu_cau_hoi", x => x.id);
                    table.ForeignKey(
                        name: "FK_hop_thu_cau_hoi_doi_tac_doi_tac_id",
                        column: x => x.doi_tac_id,
                        principalTable: "doi_tac",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_hop_thu_cau_hoi_nguoi_dung_khach_hang_id",
                        column: x => x.khach_hang_id,
                        principalTable: "nguoi_dung",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hop_thu_cau_hoi_phong_phong_id",
                        column: x => x.phong_id,
                        principalTable: "phong",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_hop_thu_cau_hoi_doi_tac_id_trang_thai",
                table: "hop_thu_cau_hoi",
                columns: new[] { "doi_tac_id", "trang_thai" });

            migrationBuilder.CreateIndex(
                name: "IX_hop_thu_cau_hoi_khach_hang_id",
                table: "hop_thu_cau_hoi",
                column: "khach_hang_id");

            migrationBuilder.CreateIndex(
                name: "IX_hop_thu_cau_hoi_phong_id",
                table: "hop_thu_cau_hoi",
                column: "phong_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hop_thu_cau_hoi");
        }
    }
}
