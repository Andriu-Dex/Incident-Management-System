using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentsTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    IncidentType = table.Column<int>(type: "int", nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedResolutionTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginIncidentId = table.Column<int>(type: "int", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticles_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticles_Incidents_OriginIncidentId",
                        column: x => x.OriginIncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticles_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleKeywords_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentArticleLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    LinkedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WasHelpful = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentArticleLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentArticleLinks_AspNetUsers_LinkedByUserId",
                        column: x => x.LinkedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentArticleLinks_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentArticleLinks_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionSteps_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleKeywords_ArticleId",
                table: "ArticleKeywords",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleKeywords_Keyword",
                table: "ArticleKeywords",
                column: "Keyword");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentArticleLinks_ArticleId",
                table: "IncidentArticleLinks",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentArticleLinks_IncidentId_ArticleId",
                table: "IncidentArticleLinks",
                columns: new[] { "IncidentId", "ArticleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentArticleLinks_LinkedByUserId",
                table: "IncidentArticleLinks",
                column: "LinkedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_AuthorId",
                table: "KnowledgeArticles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_IncidentType",
                table: "KnowledgeArticles",
                column: "IncidentType");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_IsActive",
                table: "KnowledgeArticles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_OriginIncidentId",
                table: "KnowledgeArticles",
                column: "OriginIncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_ServiceId",
                table: "KnowledgeArticles",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionSteps_ArticleId",
                table: "SolutionSteps",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleKeywords");

            migrationBuilder.DropTable(
                name: "IncidentArticleLinks");

            migrationBuilder.DropTable(
                name: "SolutionSteps");

            migrationBuilder.DropTable(
                name: "KnowledgeArticles");
        }
    }
}
