using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class SimplifyUnsubscriptionTimeline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MatchedMessageRecordId",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MatchedMessageSentAt",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MatchedMessageSnapshot",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "UnsubscriptionTimelines");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "UnsubscriptionTimelines",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "MatchedMessageRecordId",
                table: "UnsubscriptionTimelines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MatchedMessageSentAt",
                table: "UnsubscriptionTimelines",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchedMessageSnapshot",
                table: "UnsubscriptionTimelines",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "UnsubscriptionTimelines",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "UnsubscriptionTimelines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
