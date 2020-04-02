using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VegankoService.Data;

namespace VegankoService.Migrations
{
    [DbContext(typeof(VegankoContext))]
    [Migration("DBDefaultCharsetMigration")]
    public class DBDefaultCharsetMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set the default charset and collation to utfmb4 (emoji support)
            migrationBuilder.Sql("alter database character set utf8mb4 collate utf8mb4_unicode_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No going back ? :D should be reset to default charset for DB for that OS
            base.Down(migrationBuilder);
        }
    }
}
