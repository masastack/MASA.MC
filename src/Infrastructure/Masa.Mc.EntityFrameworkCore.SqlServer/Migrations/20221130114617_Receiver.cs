using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class Receiver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecords_Channels_ChannelId",
                table: "MessageRecords");

            migrationBuilder.DropTable(
                name: "ReceiverGroupUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiverGroupItems",
                table: "ReceiverGroupItems");

            migrationBuilder.DropIndex(
                name: "IX_ReceiverGroupItems_GroupId",
                table: "ReceiverGroupItems");

            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "MessageReceiverUsers");

            migrationBuilder.RenameIndex(
                name: "index_state_timessent_modificationtime",
                table: "IntegrationEventLog",
                newName: "IX_State_TimesSent_MTime");

            migrationBuilder.RenameIndex(
                name: "index_state_modificationtime",
                table: "IntegrationEventLog",
                newName: "IX_State_MTime");

            migrationBuilder.RenameIndex(
                name: "index_eventid_version",
                table: "IntegrationEventLog",
                newName: "IX_EventId_Version");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "ReceiverGroupItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ChannelType",
                table: "MessageRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChannelUserIdentity",
                table: "MessageRecords",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "MessageReceiverUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiverGroupItems",
                table: "ReceiverGroupItems",
                columns: new[] { "GroupId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiverGroupItems",
                table: "ReceiverGroupItems");

            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "MessageRecords");

            migrationBuilder.DropColumn(
                name: "ChannelUserIdentity",
                table: "MessageRecords");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "MessageReceiverUsers");

            migrationBuilder.RenameIndex(
                name: "IX_State_TimesSent_MTime",
                table: "IntegrationEventLog",
                newName: "index_state_timessent_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_State_MTime",
                table: "IntegrationEventLog",
                newName: "index_state_modificationtime");

            migrationBuilder.RenameIndex(
                name: "IX_EventId_Version",
                table: "IntegrationEventLog",
                newName: "index_eventid_version");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "ReceiverGroupItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "MessageReceiverUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiverGroupItems",
                table: "ReceiverGroupItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReceiverGroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiverGroupUsers_ReceiverGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ReceiverGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupItems_GroupId",
                table: "ReceiverGroupItems",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupUsers_GroupId_UserId",
                table: "ReceiverGroupUsers",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecords_Channels_ChannelId",
                table: "MessageRecords",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
