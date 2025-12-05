using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentsTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailNotificationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailNotificationsEnabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ThemePreference",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailNotificationsEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ThemePreference",
                table: "AspNetUsers");
        }
    }
}
