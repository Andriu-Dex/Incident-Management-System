using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentsTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEscalatedByUserIdAndUpdateLevelNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EscalatedByUserId",
                table: "Incidents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EscalatedByUserId",
                table: "Incidents",
                column: "EscalatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AspNetUsers_EscalatedByUserId",
                table: "Incidents",
                column: "EscalatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AspNetUsers_EscalatedByUserId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EscalatedByUserId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EscalatedByUserId",
                table: "Incidents");
        }
    }
}
