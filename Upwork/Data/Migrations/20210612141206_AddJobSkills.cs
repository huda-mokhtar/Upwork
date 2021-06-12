using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class AddJobSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobsSkills",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    skillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsSkills", x => new { x.JobsId, x.skillId });
                    table.ForeignKey(
                        name: "FK_JobsSkills_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsSkills_Skills_skillId",
                        column: x => x.skillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobsSkills_skillId",
                table: "JobsSkills",
                column: "skillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsSkills");
        }
    }
}
