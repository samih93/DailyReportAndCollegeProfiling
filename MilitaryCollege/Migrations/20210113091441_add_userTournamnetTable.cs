using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class add_userTournamnetTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Tournaments");

            migrationBuilder.CreateTable(
                name: "UserTournaments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    TournamentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTournaments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTournaments");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
