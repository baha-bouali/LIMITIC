using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ResearchAxes_ResearchAxisEntityId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ResearchAxisEntityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "ResearchAxisEntityId",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Publications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ResearchAxisEntityId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ResearchAxisEntityId",
                table: "Events",
                column: "ResearchAxisEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ResearchAxes_ResearchAxisEntityId",
                table: "Events",
                column: "ResearchAxisEntityId",
                principalTable: "ResearchAxes",
                principalColumn: "Id");
        }
    }
}
