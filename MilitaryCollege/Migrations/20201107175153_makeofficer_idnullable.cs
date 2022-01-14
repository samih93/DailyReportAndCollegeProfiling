using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class makeofficer_idnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyIncidents_Officers_OfficerId",
                table: "DailyIncidents");

            migrationBuilder.AlterColumn<int>(
                name: "OfficerId",
                table: "DailyIncidents",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyIncidents_Officers_OfficerId",
                table: "DailyIncidents",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyIncidents_Officers_OfficerId",
                table: "DailyIncidents");

            migrationBuilder.AlterColumn<int>(
                name: "OfficerId",
                table: "DailyIncidents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyIncidents_Officers_OfficerId",
                table: "DailyIncidents",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
