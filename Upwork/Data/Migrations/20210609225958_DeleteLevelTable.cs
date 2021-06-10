using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class DeleteLevelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectLevels");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.AddColumn<int>(
                name: "AdvancedDeliverDays",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdvancedPrice",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardDeliverDays",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardPrice",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StarterDeliverDays",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StarterPrice",
                table: "Projects",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvancedDeliverDays",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AdvancedPrice",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StandardDeliverDays",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StandardPrice",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StarterDeliverDays",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StarterPrice",
                table: "Projects");

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.LevelId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLevels",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    DeleverDays = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLevels", x => new { x.ProjectId, x.LevelId });
                    table.ForeignKey(
                        name: "FK_ProjectLevels_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLevels_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLevels_LevelId",
                table: "ProjectLevels",
                column: "LevelId");
        }
    }
}
