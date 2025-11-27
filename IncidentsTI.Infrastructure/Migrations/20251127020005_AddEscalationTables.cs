using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentsTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEscalationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentEscalationLevelId",
                table: "Incidents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EscalationLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidentEscalations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FromLevelId = table.Column<int>(type: "int", nullable: true),
                    ToLevelId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentEscalations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentEscalations_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentEscalations_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentEscalations_EscalationLevels_FromLevelId",
                        column: x => x.FromLevelId,
                        principalTable: "EscalationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentEscalations_EscalationLevels_ToLevelId",
                        column: x => x.ToLevelId,
                        principalTable: "EscalationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentEscalations_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CurrentEscalationLevelId",
                table: "Incidents",
                column: "CurrentEscalationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevels_IsActive",
                table: "EscalationLevels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationLevels_Order",
                table: "EscalationLevels",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_EscalatedAt",
                table: "IncidentEscalations",
                column: "EscalatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_FromLevelId",
                table: "IncidentEscalations",
                column: "FromLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_FromUserId",
                table: "IncidentEscalations",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_IncidentId",
                table: "IncidentEscalations",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_ToLevelId",
                table: "IncidentEscalations",
                column: "ToLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEscalations_ToUserId",
                table: "IncidentEscalations",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_EscalationLevels_CurrentEscalationLevelId",
                table: "Incidents",
                column: "CurrentEscalationLevelId",
                principalTable: "EscalationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_EscalationLevels_CurrentEscalationLevelId",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "IncidentEscalations");

            migrationBuilder.DropTable(
                name: "EscalationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_CurrentEscalationLevelId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "CurrentEscalationLevelId",
                table: "Incidents");
        }
    }
}
