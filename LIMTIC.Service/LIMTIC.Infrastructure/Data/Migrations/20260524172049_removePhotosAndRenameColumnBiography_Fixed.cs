using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removePhotosAndRenameColumnBiography_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Speakers");

            migrationBuilder.RenameColumn(
                name: "Biography",
                table: "Speakers",
                newName: "Subject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Speakers",
                newName: "Biography");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Speakers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
