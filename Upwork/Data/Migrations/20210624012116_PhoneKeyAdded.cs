using Microsoft.EntityFrameworkCore.Migrations;

namespace Upwork.Data.Migrations
{
    public partial class PhoneKeyAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PhoneCountryId",
                table: "Freelancers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SendMe",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Freelancers_PhoneCountryId",
                table: "Freelancers",
                column: "PhoneCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancers_Countries_PhoneCountryId",
                table: "Freelancers",
                column: "PhoneCountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Freelancers_Countries_PhoneCountryId",
                table: "Freelancers");

            migrationBuilder.DropIndex(
                name: "IX_Freelancers_PhoneCountryId",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "PhoneCountryId",
                table: "Freelancers");

            migrationBuilder.AlterColumn<bool>(
                name: "SendMe",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
