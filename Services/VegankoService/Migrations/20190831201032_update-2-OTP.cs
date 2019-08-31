using Microsoft.EntityFrameworkCore.Migrations;

namespace VegankoService.Migrations
{
    public partial class update2OTP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoginCount",
                table: "OTPs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginCount",
                table: "OTPs");
        }
    }
}
