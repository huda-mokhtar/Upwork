using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class JobDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostAJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    subCategoryId = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    LevelOfExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfBudget = table.Column<bool>(type: "bit", nullable: false),
                    BudgetFrom = table.Column<int>(type: "int", nullable: false),
                    BudgetTo = table.Column<int>(type: "int", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language_ProficiencyId = table.Column<int>(type: "int", nullable: false),
                    TimeRequirement = table.Column<bool>(type: "bit", nullable: true),
                    TalentType = table.Column<bool>(type: "bit", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostAJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostAJobs_Language_Proficiency_Language_ProficiencyId",
                        column: x => x.Language_ProficiencyId,
                        principalTable: "Language_Proficiency",
                        principalColumn: "ProficiencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostAJobs_SubCategories_subCategoryId",
                        column: x => x.subCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "SubCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                columns: table => new
                {
                    PostAJobId = table.Column<int>(type: "int", nullable: false),
                    skillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkills", x => new { x.skillId, x.PostAJobId });
                    table.ForeignKey(
                        name: "FK_JobSkills_PostAJobs_PostAJobId",
                        column: x => x.PostAJobId,
                        principalTable: "PostAJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkills_Skills_skillId",
                        column: x => x.skillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_PostAJobId",
                table: "JobSkills",
                column: "PostAJobId");

            migrationBuilder.CreateIndex(
                name: "IX_PostAJobs_Language_ProficiencyId",
                table: "PostAJobs",
                column: "Language_ProficiencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PostAJobs_subCategoryId",
                table: "PostAJobs",
                column: "subCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "PostAJobs");
        }
    }
}
