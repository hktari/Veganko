using Microsoft.EntityFrameworkCore.Migrations;

namespace VegankoService.Migrations
{
    public partial class AddsNewlyCreatedProductId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewlyCreatedProductId",
                table: "ProductModRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewlyCreatedProductId",
                table: "ProductModRequests");
        }
    }
}
