using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class AddJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    subCategoryId = table.Column<int>(type: "int", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    LevelOfExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfBudget = table.Column<bool>(type: "bit", nullable: false),
                    BudgetFrom = table.Column<int>(type: "int", nullable: true),
                    BudgetTo = table.Column<int>(type: "int", nullable: true),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language_ProficiencyId = table.Column<int>(type: "int", nullable: true),
                    TimeRequirement = table.Column<bool>(type: "bit", nullable: false),
                    TalentType = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Language_Proficiency_Language_ProficiencyId",
                        column: x => x.Language_ProficiencyId,
                        principalTable: "Language_Proficiency",
                        principalColumn: "ProficiencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_SubCategories_subCategoryId",
                        column: x => x.subCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "SubCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Language_ProficiencyId",
                table: "Jobs",
                column: "Language_ProficiencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_subCategoryId",
                table: "Jobs",
                column: "subCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Projects");

        }  

    }
}
