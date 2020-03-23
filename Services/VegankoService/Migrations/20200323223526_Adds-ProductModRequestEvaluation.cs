using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VegankoService.Migrations
{
    public partial class AddsProductModRequestEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChangedFields",
                table: "ProductModRequests",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ProductModRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductModRequestEvaluations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EvaluatorUserId = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ProductModRequestId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModRequestEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductModRequestEvaluations_ProductModRequests_ProductModRe~",
                        column: x => x.ProductModRequestId,
                        principalTable: "ProductModRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductModRequestEvaluations_ProductModRequestId",
                table: "ProductModRequestEvaluations",
                column: "ProductModRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductModRequestEvaluations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ProductModRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ChangedFields",
                table: "ProductModRequests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
