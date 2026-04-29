using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreatedByType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users"" ALTER COLUMN ""CreatedBy"" TYPE uuid USING (""CreatedBy""::text::uuid);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users"" ALTER COLUMN ""CreatedBy"" TYPE integer USING (""CreatedBy""::text::integer);"
            );
            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
