using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class makeofficernullableindailynote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyNotes_Officers_OfficerId",
                table: "DailyNotes");

            migrationBuilder.AlterColumn<int>(
                name: "OfficerId",
                table: "DailyNotes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyNotes_Officers_OfficerId",
                table: "DailyNotes",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyNotes_Officers_OfficerId",
                table: "DailyNotes");

            migrationBuilder.AlterColumn<int>(
                name: "OfficerId",
                table: "DailyNotes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyNotes_Officers_OfficerId",
                table: "DailyNotes",
                column: "OfficerId",
                principalTable: "Officers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
