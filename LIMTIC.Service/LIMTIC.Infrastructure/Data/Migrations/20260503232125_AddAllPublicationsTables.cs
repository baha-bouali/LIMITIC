using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAllPublicationsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookChapterEntity_PublicationEntity_PublicationId",
                table: "BookChapterEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_InternationalConferenceEntity_PublicationEntity_Publication~",
                table: "InternationalConferenceEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalArticleEntity_PublicationEntity_PublicationId",
                table: "JournalArticleEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalConferenceEntity_PublicationEntity_PublicationId",
                table: "NationalConferenceEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicationEntity_ResearchAxes_ResearchAxisId",
                table: "PublicationEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicationEntity_Users_UserId",
                table: "PublicationEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalReportEntity_PublicationEntity_PublicationId",
                table: "TechnicalReportEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicalReportEntity",
                table: "TechnicalReportEntity");

            migrationBuilder.DropIndex(
                name: "IX_TechnicalReportEntity_PublicationId",
                table: "TechnicalReportEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicationEntity",
                table: "PublicationEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NationalConferenceEntity",
                table: "NationalConferenceEntity");

            migrationBuilder.DropIndex(
                name: "IX_NationalConferenceEntity_PublicationId",
                table: "NationalConferenceEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalArticleEntity",
                table: "JournalArticleEntity");

            migrationBuilder.DropIndex(
                name: "IX_JournalArticleEntity_PublicationId",
                table: "JournalArticleEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternationalConferenceEntity",
                table: "InternationalConferenceEntity");

            migrationBuilder.DropIndex(
                name: "IX_InternationalConferenceEntity_PublicationId",
                table: "InternationalConferenceEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapterEntity",
                table: "BookChapterEntity");

            migrationBuilder.DropIndex(
                name: "IX_BookChapterEntity_PublicationId",
                table: "BookChapterEntity");

            migrationBuilder.RenameTable(
                name: "TechnicalReportEntity",
                newName: "TechnicalReports");

            migrationBuilder.RenameTable(
                name: "PublicationEntity",
                newName: "Publications");

            migrationBuilder.RenameTable(
                name: "NationalConferenceEntity",
                newName: "NationalConferences");

            migrationBuilder.RenameTable(
                name: "JournalArticleEntity",
                newName: "JournalArticles");

            migrationBuilder.RenameTable(
                name: "InternationalConferenceEntity",
                newName: "InternationalConferences");

            migrationBuilder.RenameTable(
                name: "BookChapterEntity",
                newName: "BookChapters");

            migrationBuilder.RenameIndex(
                name: "IX_PublicationEntity_UserId",
                table: "Publications",
                newName: "IX_Publications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicationEntity_ResearchAxisId",
                table: "Publications",
                newName: "IX_Publications_ResearchAxisId");

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "TechnicalReports",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Visibility",
                table: "Publications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Publications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Publications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Publications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "Publications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "NationalConferences",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "NationalConferences",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ConferenceName",
                table: "NationalConferences",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Volume",
                table: "JournalArticles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Ranking",
                table: "JournalArticles",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "JournalArticles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "JournalArticles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "JournalName",
                table: "JournalArticles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Ranking",
                table: "InternationalConferences",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "InternationalConferences",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "InternationalConferences",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ConferenceName",
                table: "InternationalConferences",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Publisher",
                table: "BookChapters",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "BookChapters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "BookChapters",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "BookChapters",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publications",
                table: "Publications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalArticles",
                table: "JournalArticles",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternationalConferences",
                table: "InternationalConferences",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters",
                column: "PublicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookChapters_Publications_PublicationId",
                table: "BookChapters",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternationalConferences_Publications_PublicationId",
                table: "InternationalConferences",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalArticles_Publications_PublicationId",
                table: "JournalArticles",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NationalConferences_Publications_PublicationId",
                table: "NationalConferences",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_ResearchAxes_ResearchAxisId",
                table: "Publications",
                column: "ResearchAxisId",
                principalTable: "ResearchAxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalReports_Publications_PublicationId",
                table: "TechnicalReports",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookChapters_Publications_PublicationId",
                table: "BookChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_InternationalConferences_Publications_PublicationId",
                table: "InternationalConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalArticles_Publications_PublicationId",
                table: "JournalArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalConferences_Publications_PublicationId",
                table: "NationalConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_Publications_ResearchAxes_ResearchAxisId",
                table: "Publications");

            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalReports_Publications_PublicationId",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publications",
                table: "Publications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalArticles",
                table: "JournalArticles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternationalConferences",
                table: "InternationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters");

            migrationBuilder.RenameTable(
                name: "TechnicalReports",
                newName: "TechnicalReportEntity");

            migrationBuilder.RenameTable(
                name: "Publications",
                newName: "PublicationEntity");

            migrationBuilder.RenameTable(
                name: "NationalConferences",
                newName: "NationalConferenceEntity");

            migrationBuilder.RenameTable(
                name: "JournalArticles",
                newName: "JournalArticleEntity");

            migrationBuilder.RenameTable(
                name: "InternationalConferences",
                newName: "InternationalConferenceEntity");

            migrationBuilder.RenameTable(
                name: "BookChapters",
                newName: "BookChapterEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Publications_UserId",
                table: "PublicationEntity",
                newName: "IX_PublicationEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Publications_ResearchAxisId",
                table: "PublicationEntity",
                newName: "IX_PublicationEntity_ResearchAxisId");

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "TechnicalReportEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                table: "PublicationEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "PublicationEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PublicationEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PublicationEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string[]>(
                name: "Keywords",
                table: "PublicationEntity",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "NationalConferenceEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "NationalConferenceEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "ConferenceName",
                table: "NationalConferenceEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Volume",
                table: "JournalArticleEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Ranking",
                table: "JournalArticleEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "JournalArticleEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "JournalArticleEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "JournalName",
                table: "JournalArticleEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Ranking",
                table: "InternationalConferenceEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "InternationalConferenceEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "InternationalConferenceEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "ConferenceName",
                table: "InternationalConferenceEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Publisher",
                table: "BookChapterEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Pages",
                table: "BookChapterEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "BookChapterEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "BookChapterEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicalReportEntity",
                table: "TechnicalReportEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicationEntity",
                table: "PublicationEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NationalConferenceEntity",
                table: "NationalConferenceEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalArticleEntity",
                table: "JournalArticleEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternationalConferenceEntity",
                table: "InternationalConferenceEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookChapterEntity",
                table: "BookChapterEntity",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalReportEntity_PublicationId",
                table: "TechnicalReportEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NationalConferenceEntity_PublicationId",
                table: "NationalConferenceEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalArticleEntity_PublicationId",
                table: "JournalArticleEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternationalConferenceEntity_PublicationId",
                table: "InternationalConferenceEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterEntity_PublicationId",
                table: "BookChapterEntity",
                column: "PublicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookChapterEntity_PublicationEntity_PublicationId",
                table: "BookChapterEntity",
                column: "PublicationId",
                principalTable: "PublicationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternationalConferenceEntity_PublicationEntity_Publication~",
                table: "InternationalConferenceEntity",
                column: "PublicationId",
                principalTable: "PublicationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalArticleEntity_PublicationEntity_PublicationId",
                table: "JournalArticleEntity",
                column: "PublicationId",
                principalTable: "PublicationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NationalConferenceEntity_PublicationEntity_PublicationId",
                table: "NationalConferenceEntity",
                column: "PublicationId",
                principalTable: "PublicationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicationEntity_ResearchAxes_ResearchAxisId",
                table: "PublicationEntity",
                column: "ResearchAxisId",
                principalTable: "ResearchAxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicationEntity_Users_UserId",
                table: "PublicationEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalReportEntity_PublicationEntity_PublicationId",
                table: "TechnicalReportEntity",
                column: "PublicationId",
                principalTable: "PublicationEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
