using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResearchAxisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearchAxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Themes = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchAxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Program = table.Column<string>(type: "text", nullable: true),
                    PhotoFileNames = table.Column<List<string>>(type: "text[]", nullable: false),
                    ResearchAxisId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventEntity_ResearchAxes_ResearchAxisId",
                        column: x => x.ResearchAxisId,
                        principalTable: "ResearchAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicationEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResearchAxisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Abstract = table.Column<string>(type: "text", nullable: false),
                    Keywords = table.Column<string[]>(type: "text[]", nullable: false),
                    AttachedPdfs = table.Column<string[]>(type: "text[]", nullable: false),
                    Doi = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicationEntity_ResearchAxes_ResearchAxisId",
                        column: x => x.ResearchAxisId,
                        principalTable: "ResearchAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicationEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeakerEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeakerEntity_EventEntity_EventId",
                        column: x => x.EventId,
                        principalTable: "EventEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookChapterEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookTitle = table.Column<string>(type: "text", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: false),
                    Isbn = table.Column<string>(type: "text", nullable: true),
                    Pages = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookChapterEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookChapterEntity_PublicationEntity_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "PublicationEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InternationalConferenceEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConferenceName = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Pages = table.Column<string>(type: "text", nullable: true),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternationalConferenceEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternationalConferenceEntity_PublicationEntity_Publication~",
                        column: x => x.PublicationId,
                        principalTable: "PublicationEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalArticleEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalName = table.Column<string>(type: "text", nullable: false),
                    Volume = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Pages = table.Column<string>(type: "text", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalArticleEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalArticleEntity_PublicationEntity_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "PublicationEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NationalConferenceEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConferenceName = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Pages = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NationalConferenceEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NationalConferenceEntity_PublicationEntity_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "PublicationEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalReportEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportNumber = table.Column<long>(type: "bigint", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalReportEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalReportEntity_PublicationEntity_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "PublicationEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterEntity_PublicationId",
                table: "BookChapterEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventEntity_ResearchAxisId",
                table: "EventEntity",
                column: "ResearchAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalConferenceEntity_PublicationId",
                table: "InternationalConferenceEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalArticleEntity_PublicationId",
                table: "JournalArticleEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NationalConferenceEntity_PublicationId",
                table: "NationalConferenceEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicationEntity_ResearchAxisId",
                table: "PublicationEntity",
                column: "ResearchAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationEntity_UserId",
                table: "PublicationEntity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerEntity_EventId",
                table: "SpeakerEntity",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalReportEntity_PublicationId",
                table: "TechnicalReportEntity",
                column: "PublicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookChapterEntity");

            migrationBuilder.DropTable(
                name: "InternationalConferenceEntity");

            migrationBuilder.DropTable(
                name: "JournalArticleEntity");

            migrationBuilder.DropTable(
                name: "NationalConferenceEntity");

            migrationBuilder.DropTable(
                name: "SpeakerEntity");

            migrationBuilder.DropTable(
                name: "TechnicalReportEntity");

            migrationBuilder.DropTable(
                name: "EventEntity");

            migrationBuilder.DropTable(
                name: "PublicationEntity");

            migrationBuilder.DropTable(
                name: "ResearchAxes");
        }
    }
}
