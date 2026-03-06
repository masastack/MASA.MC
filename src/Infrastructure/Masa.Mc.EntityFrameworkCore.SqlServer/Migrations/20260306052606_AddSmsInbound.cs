using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class AddSmsInbound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId",
                table: "MessageReceiverUsers");

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SmsInbounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SmsContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AddSerial = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsInbounds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId_ChannelUserIdentity_Platform",
                table: "MessageReceiverUsers",
                columns: new[] { "MessageTaskHistoryId", "ChannelUserIdentity", "Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_ChannelId",
                table: "SmsInbounds",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Mobile",
                table: "SmsInbounds",
                column: "Mobile");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Provider",
                table: "SmsInbounds",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_SendTime",
                table: "SmsInbounds",
                column: "SendTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsInbounds");

            migrationBuilder.DropIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId_ChannelUserIdentity_Platform",
                table: "MessageReceiverUsers");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Channels");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId",
                table: "MessageReceiverUsers",
                column: "MessageTaskHistoryId");
        }
    }
}
