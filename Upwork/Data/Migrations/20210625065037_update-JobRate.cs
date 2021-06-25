using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class updateJobRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Freelancer_Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Freelancer_Jobs");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
