using Microsoft.EntityFrameworkCore.Migrations;

namespace MilitaryCollege.Migrations
{
    public partial class AddReasonDbset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReasonOfIncidentId",
                table: "DailyIncidents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReasonOfIncidents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonOfIncidents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyIncidents_ReasonOfIncidentId",
                table: "DailyIncidents",
                column: "ReasonOfIncidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyIncidents_ReasonOfIncidents_ReasonOfIncidentId",
                table: "DailyIncidents",
                column: "ReasonOfIncidentId",
                principalTable: "ReasonOfIncidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyIncidents_ReasonOfIncidents_ReasonOfIncidentId",
                table: "DailyIncidents");

            migrationBuilder.DropTable(
                name: "ReasonOfIncidents");

            migrationBuilder.DropIndex(
                name: "IX_DailyIncidents_ReasonOfIncidentId",
                table: "DailyIncidents");

            migrationBuilder.DropColumn(
                name: "ReasonOfIncidentId",
                table: "DailyIncidents");
        }
    }
}
