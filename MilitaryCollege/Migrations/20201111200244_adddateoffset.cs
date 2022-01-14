using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class adddateoffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartDate",
                table: "DailyIncidents",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndDate",
                table: "DailyIncidents",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DailyIncidents",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "DailyIncidents",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);
        }
    }
}
