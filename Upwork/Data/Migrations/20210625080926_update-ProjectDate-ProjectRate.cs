using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class updateProjectDateProjectRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Projects",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Client_Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Client_Projects");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Projects",
                newName: "Data");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
