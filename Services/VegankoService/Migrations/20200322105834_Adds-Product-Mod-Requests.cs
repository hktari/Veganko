using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VegankoService.Migrations
{
    public partial class AddsProductModRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Product");

            migrationBuilder.CreateTable(
                name: "UnapprovedProducts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    ImageName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ProductClassifiers = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    AddedTimestamp = table.Column<DateTime>(nullable: false),
                    LastUpdateTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnapprovedProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductModRequests",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ExistingProductId = table.Column<string>(nullable: true),
                    Action = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UnapprovedProductId = table.Column<string>(nullable: false),
                    ChangedFields = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductModRequests_UnapprovedProducts_UnapprovedProductId",
                        column: x => x.UnapprovedProductId,
                        principalTable: "UnapprovedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductModRequests_UnapprovedProductId",
                table: "ProductModRequests",
                column: "UnapprovedProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductModRequests");

            migrationBuilder.DropTable(
                name: "UnapprovedProducts");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Product",
                nullable: false,
                defaultValue: 0);
        }
    }
}
