using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class Updatedurationtostartdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentPerDays");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "DailyIncidents");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "DailyIncidents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "DailyIncidents");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "DailyIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncidentPerDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyIncidentId = table.Column<int>(type: "int", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TournamentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentPerDays", x => x.Id);
                });
        }
    }
}
