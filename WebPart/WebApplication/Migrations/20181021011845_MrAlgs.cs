using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class MrAlgs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MrAlgorithms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MrAlgorithms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MrAnalyzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    MrRecordId = table.Column<Guid>(nullable: false),
                    MrAlgorithmId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MrAnalyzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MrAnalyzes_MrAlgorithms_MrAlgorithmId",
                        column: x => x.MrAlgorithmId,
                        principalTable: "MrAlgorithms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MrAnalyzes_MrRecords_MrRecordId",
                        column: x => x.MrRecordId,
                        principalTable: "MrRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MrAnalyzes_MrAlgorithmId",
                table: "MrAnalyzes",
                column: "MrAlgorithmId");

            migrationBuilder.CreateIndex(
                name: "IX_MrAnalyzes_MrRecordId",
                table: "MrAnalyzes",
                column: "MrRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MrAnalyzes");

            migrationBuilder.DropTable(
                name: "MrAlgorithms");
        }
    }
}
