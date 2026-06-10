using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class AddUnsubscriptionTimelineSnapshotColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchedMessageRecordId",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MatchedMessageSentAt",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropColumn(
                name: "MatchedMessageSnapshot",
                table: "UnsubscriptionTimelines");
        }
    }
}
