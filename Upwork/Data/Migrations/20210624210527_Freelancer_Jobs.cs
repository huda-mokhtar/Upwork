using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class Freelancer_Jobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreelancerSavedJobs");

            migrationBuilder.CreateTable(
                name: "Freelancer_Jobs",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    FreelancerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSaved = table.Column<bool>(type: "bit", nullable: true),
                    IsProposal = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Freelancer_Jobs", x => new { x.JobsId, x.FreelancerId });
                    table.ForeignKey(
                        name: "FK_Freelancer_Jobs_Freelancers_FreelancerId",
                        column: x => x.FreelancerId,
                        principalTable: "Freelancers",
                        principalColumn: "FreelancerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Freelancer_Jobs_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Freelancer_Jobs_FreelancerId",
                table: "Freelancer_Jobs",
                column: "FreelancerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Freelancer_Jobs");

            migrationBuilder.CreateTable(
                name: "FreelancerSavedJobs",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    FreelancerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelancerSavedJobs", x => new { x.JobsId, x.FreelancerId });
                    table.ForeignKey(
                        name: "FK_FreelancerSavedJobs_Freelancers_FreelancerId",
                        column: x => x.FreelancerId,
                        principalTable: "Freelancers",
                        principalColumn: "FreelancerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FreelancerSavedJobs_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerSavedJobs_FreelancerId",
                table: "FreelancerSavedJobs",
                column: "FreelancerId");
        }
    }
}
