using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class subcategoryskills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SubCategoryId",
                table: "Skills",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_SubCategories_SubCategoryId",
                table: "Skills",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "SubCategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_SubCategories_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_SubCategoryId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Skills");
        }
    }
}
