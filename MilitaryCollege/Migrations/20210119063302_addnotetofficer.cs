using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class addnotetofficer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Officers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Officers");
        }
    }
}
