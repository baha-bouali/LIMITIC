using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResearchAxesToProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswords_User_UserId",
                table: "ResetPasswords");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.AlterColumn<string>(
                name: "ThesisSubject",
                table: "PhDStudents",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "PhDStudents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "Masterians",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "PhDStudentEntityResearchAxisEntity",
                columns: table => new
                {
                    PhDStudentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResearchAxesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhDStudentEntityResearchAxisEntity", x => new { x.PhDStudentsId, x.ResearchAxesId });
                    table.ForeignKey(
                        name: "FK_PhDStudentEntityResearchAxisEntity_PhDStudents_PhDStudentsId",
                        column: x => x.PhDStudentsId,
                        principalTable: "PhDStudents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhDStudentEntityResearchAxisEntity_ResearchAxes_ResearchAxe~",
                        column: x => x.ResearchAxesId,
                        principalTable: "ResearchAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResearchAxisEntityResearcherEntity",
                columns: table => new
                {
                    ResearchAxesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResearchersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchAxisEntityResearcherEntity", x => new { x.ResearchAxesId, x.ResearchersId });
                    table.ForeignKey(
                        name: "FK_ResearchAxisEntityResearcherEntity_ResearchAxes_ResearchAxe~",
                        column: x => x.ResearchAxesId,
                        principalTable: "ResearchAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResearchAxisEntityResearcherEntity_Researchers_ResearchersId",
                        column: x => x.ResearchersId,
                        principalTable: "Researchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhDStudentEntityResearchAxisEntity_ResearchAxesId",
                table: "PhDStudentEntityResearchAxisEntity",
                column: "ResearchAxesId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchAxisEntityResearcherEntity_ResearchersId",
                table: "ResearchAxisEntityResearcherEntity",
                column: "ResearchersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswords_Users_UserId",
                table: "ResetPasswords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswords_Users_UserId",
                table: "ResetPasswords");

            migrationBuilder.DropTable(
                name: "PhDStudentEntityResearchAxisEntity");

            migrationBuilder.DropTable(
                name: "ResearchAxisEntityResearcherEntity");

            migrationBuilder.AlterColumn<string>(
                name: "ThesisSubject",
                table: "PhDStudents",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "PhDStudents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SupervisorId",
                table: "Masterians",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    TempId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_User_TempId1", x => x.TempId1);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswords_User_UserId",
                table: "ResetPasswords",
                column: "UserId",
                principalTable: "User",
                principalColumn: "TempId1",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
