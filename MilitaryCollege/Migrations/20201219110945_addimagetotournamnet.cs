using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class addimagetotournamnet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "DailyIncidents");

            migrationBuilder.AddColumn<string>(
                name: "TournamentImage",
                table: "Tournaments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentImage",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "DailyIncidents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
