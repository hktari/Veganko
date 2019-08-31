using Microsoft.EntityFrameworkCore.Migrations;

namespace VegankoService.Migrations
{
    public partial class updateOTP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "OTPs",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OTPs",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
