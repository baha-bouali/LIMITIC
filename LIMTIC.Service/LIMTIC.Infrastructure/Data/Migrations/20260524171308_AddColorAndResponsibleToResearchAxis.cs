using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColorAndResponsibleToResearchAxis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ResearchAxes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleId",
                table: "ResearchAxes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResearchAxes_ResponsibleId",
                table: "ResearchAxes",
                column: "ResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResearchAxes_Researchers_ResponsibleId",
                table: "ResearchAxes",
                column: "ResponsibleId",
                principalTable: "Researchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResearchAxes_Researchers_ResponsibleId",
                table: "ResearchAxes");

            migrationBuilder.DropIndex(
                name: "IX_ResearchAxes_ResponsibleId",
                table: "ResearchAxes");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ResearchAxes");

            migrationBuilder.DropColumn(
                name: "ResponsibleId",
                table: "ResearchAxes");
        }
    }
}
