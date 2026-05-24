using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLabSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LabName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LabSlogan = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SmtpHost = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SmtpPort = table.Column<int>(type: "integer", nullable: false),
                    SmtpUsername = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SmtpPasswordHash = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SmtpUseTls = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LabSettings",
                columns: new[] { "Id", "Address", "ContactEmail", "CreatedAtUtc", "CreatedBy", "LabName", "LabSlogan", "LogoUrl", "Phone", "SmtpHost", "SmtpPasswordHash", "SmtpPort", "SmtpUseTls", "SmtpUsername" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "", "", new DateTime(2026, 5, 24, 0, 22, 22, 106, DateTimeKind.Utc).AddTicks(4560), new Guid("00000000-0000-0000-0000-000000000000"), "LIMTIC Lab", "", null, "", "", "", 25, false, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabSettings");
        }
    }
}
