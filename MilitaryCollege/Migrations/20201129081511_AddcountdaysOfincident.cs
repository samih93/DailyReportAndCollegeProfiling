using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class AddcountdaysOfincident : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountDaysOfIncident",
                table: "DailyIncidents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountDaysOfIncident",
                table: "DailyIncidents");
        }
    }
}
