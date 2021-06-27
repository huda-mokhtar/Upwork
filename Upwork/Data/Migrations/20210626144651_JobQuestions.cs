using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class JobQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewJobQuestions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewJobQuestions", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "JobQuestions",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    ReviewJobQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQuestions", x => new { x.JobsId, x.ReviewJobQuestionId });
                    table.ForeignKey(
                        name: "FK_JobQuestions_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobQuestions_ReviewJobQuestions_ReviewJobQuestionId",
                        column: x => x.ReviewJobQuestionId,
                        principalTable: "ReviewJobQuestions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobQuestions_ReviewJobQuestionId",
                table: "JobQuestions",
                column: "ReviewJobQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobQuestions");

            migrationBuilder.DropTable(
                name: "ReviewJobQuestions");
        }
    }
}
