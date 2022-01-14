using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class makeDatetimeToDailyIncident : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DailyIncidents",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "DailyIncidents",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartDate",
                table: "DailyIncidents",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndDate",
                table: "DailyIncidents",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
