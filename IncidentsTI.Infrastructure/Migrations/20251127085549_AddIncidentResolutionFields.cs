using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentsTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentResolutionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolutionDescription",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResolutionTimeMinutes",
                table: "Incidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedById",
                table: "Incidents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootCause",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ResolvedById",
                table: "Incidents",
                column: "ResolvedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AspNetUsers_ResolvedById",
                table: "Incidents",
                column: "ResolvedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AspNetUsers_ResolvedById",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_ResolvedById",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolutionDescription",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolutionTimeMinutes",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolvedById",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "RootCause",
                table: "Incidents");
        }
    }
}
