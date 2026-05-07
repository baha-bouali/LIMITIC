using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeIdPrimaryKeyInAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_Masterians_Researchers_SupervisorId",
                table: "Masterians");

            migrationBuilder.DropForeignKey(
                name: "FK_Masterians_Users_UserId",
                table: "Masterians");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalConferences_Publications_PublicationId",
                table: "NationalConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_PhDStudents_Researchers_SupervisorId",
                table: "PhDStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_PhDStudents_Users_UserId",
                table: "PhDStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Researchers_Users_UserId",
                table: "Researchers");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalReports_Publications_PublicationId",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Researchers",
                table: "Researchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhDStudents",
                table: "PhDStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Masterians",
                table: "Masterians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalArticles",
                table: "JournalArticles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternationalConferences",
                table: "InternationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "TechnicalReports");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Researchers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PhDStudents");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "NationalConferences");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Masterians");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "JournalArticles");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "InternationalConferences");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "BookChapters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Researchers",
                table: "Researchers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhDStudents",
                table: "PhDStudents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Masterians",
                table: "Masterians",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JournalArticles",
                table: "JournalArticles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InternationalConferences",
                table: "InternationalConferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookChapters_Publications_Id",
                table: "BookChapters",
                column: "Id",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternationalConferences_Publications_Id",
                table: "InternationalConferences",
                column: "Id",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalArticles_Publications_Id",
                table: "JournalArticles",
                column: "Id",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Masterians_Researchers_SupervisorId",
                table: "Masterians",
                column: "SupervisorId",
                principalTable: "Researchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Masterians_Users_Id",
                table: "Masterians",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NationalConferences_Publications_Id",
                table: "NationalConferences",
                column: "Id",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhDStudents_Researchers_SupervisorId",
                table: "PhDStudents",
                column: "SupervisorId",
                principalTable: "Researchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhDStudents_Users_Id",
                table: "PhDStudents",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Researchers_Users_Id",
                table: "Researchers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalReports_Publications_Id",
                table: "TechnicalReports",
                column: "Id",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookChapters_Publications_Id",
                table: "BookChapters");

            migrationBuilder.DropForeignKey(
                name: "FK_InternationalConferences_Publications_Id",
                table: "InternationalConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalArticles_Publications_Id",
                table: "JournalArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_Masterians_Researchers_SupervisorId",
                table: "Masterians");

            migrationBuilder.DropForeignKey(
                name: "FK_Masterians_Users_Id",
                table: "Masterians");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalConferences_Publications_Id",
                table: "NationalConferences");

            migrationBuilder.DropForeignKey(
                name: "FK_PhDStudents_Researchers_SupervisorId",
                table: "PhDStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_PhDStudents_Users_Id",
                table: "PhDStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Researchers_Users_Id",
                table: "Researchers");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalReports_Publications_Id",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Researchers",
                table: "Researchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhDStudents",
                table: "PhDStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Masterians",
                table: "Masterians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JournalArticles",
                table: "JournalArticles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InternationalConferences",
                table: "InternationalConferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookChapters",
                table: "BookChapters");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicationId",
                table: "TechnicalReports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Researchers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PhDStudents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicationId",
                table: "NationalConferences",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Masterians",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicationId",
                table: "JournalArticles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicationId",
                table: "InternationalConferences",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicationId",
                table: "BookChapters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicalReports",
                table: "TechnicalReports",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Researchers",
                table: "Researchers",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhDStudents",
                table: "PhDStudents",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NationalConferences",
                table: "NationalConferences",
                column: "PublicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Masterians",
                table: "Masterians",
                column: "UserId");

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
                name: "FK_Masterians_Researchers_SupervisorId",
                table: "Masterians",
                column: "SupervisorId",
                principalTable: "Researchers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Masterians_Users_UserId",
                table: "Masterians",
                column: "UserId",
                principalTable: "Users",
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
                name: "FK_PhDStudents_Researchers_SupervisorId",
                table: "PhDStudents",
                column: "SupervisorId",
                principalTable: "Researchers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhDStudents_Users_UserId",
                table: "PhDStudents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Researchers_Users_UserId",
                table: "Researchers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalReports_Publications_PublicationId",
                table: "TechnicalReports",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
