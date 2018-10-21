using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class MrRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MrRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HospitalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MrRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MrRecords_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MrRecords_HospitalId",
                table: "MrRecords",
                column: "HospitalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MrRecords");
        }
    }
}
