using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Events_Speakers_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventEntity_ResearchAxes_ResearchAxisId",
                table: "EventEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SpeakerEntity_EventEntity_EventId",
                table: "SpeakerEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpeakerEntity",
                table: "SpeakerEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventEntity",
                table: "EventEntity");

            migrationBuilder.RenameTable(
                name: "SpeakerEntity",
                newName: "Speakers");

            migrationBuilder.RenameTable(
                name: "EventEntity",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_SpeakerEntity_EventId",
                table: "Speakers",
                newName: "IX_Speakers_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventEntity_ResearchAxisId",
                table: "Events",
                newName: "IX_Events_ResearchAxisId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Speakers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "Speakers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Speakers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "Speakers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Speakers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Speakers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Biography",
                table: "Speakers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Program",
                table: "Events",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoFileNames",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Events",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ResearchAxisEntityId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Speakers",
                table: "Speakers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ResearchAxes_ResearchAxisId",
                table: "Events",
                column: "ResearchAxisId",
                principalTable: "ResearchAxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Speakers_Events_EventId",
                table: "Speakers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ResearchAxes_ResearchAxisEntityId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ResearchAxes_ResearchAxisId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Speakers_Events_EventId",
                table: "Speakers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Speakers",
                table: "Speakers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ResearchAxisEntityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ResearchAxisEntityId",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Speakers",
                newName: "SpeakerEntity");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "EventEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Speakers_EventId",
                table: "SpeakerEntity",
                newName: "IX_SpeakerEntity_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ResearchAxisId",
                table: "EventEntity",
                newName: "IX_EventEntity_ResearchAxisId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "SpeakerEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "SpeakerEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "SpeakerEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "SpeakerEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "SpeakerEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SpeakerEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Biography",
                table: "SpeakerEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "EventEntity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "EventEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Program",
                table: "EventEntity",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "PhotoFileNames",
                table: "EventEntity",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "EventEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EventEntity",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpeakerEntity",
                table: "SpeakerEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventEntity",
                table: "EventEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventEntity_ResearchAxes_ResearchAxisId",
                table: "EventEntity",
                column: "ResearchAxisId",
                principalTable: "ResearchAxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpeakerEntity_EventEntity_EventId",
                table: "SpeakerEntity",
                column: "EventId",
                principalTable: "EventEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
