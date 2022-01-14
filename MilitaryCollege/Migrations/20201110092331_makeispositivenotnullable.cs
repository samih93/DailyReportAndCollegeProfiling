using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class makeispositivenotnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPositive",
                table: "DailyNotes",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPositive",
                table: "DailyNotes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
