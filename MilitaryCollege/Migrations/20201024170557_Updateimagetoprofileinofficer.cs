using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class Updateimagetoprofileinofficer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Officers");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Officers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Officers");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Officers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
