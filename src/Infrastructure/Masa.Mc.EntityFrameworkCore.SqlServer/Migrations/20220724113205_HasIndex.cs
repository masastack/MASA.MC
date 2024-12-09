using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class HasIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessages_UserId_ChannelId",
                table: "WebsiteMessages",
                columns: new[] { "UserId", "ChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageCursors_UserId",
                table: "WebsiteMessageCursors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_ChannelId",
                table: "SmsTemplates",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_ChannelId",
                table: "MessageTemplates",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId",
                table: "MessageRecords",
                column: "MessageTaskHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_UserId",
                table: "MessageRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Code",
                table: "Channels",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Type",
                table: "Channels",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebsiteMessages_UserId_ChannelId",
                table: "WebsiteMessages");

            migrationBuilder.DropIndex(
                name: "IX_WebsiteMessageCursors_UserId",
                table: "WebsiteMessageCursors");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplates_ChannelId",
                table: "SmsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MessageTemplates_ChannelId",
                table: "MessageTemplates");

            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId",
                table: "MessageRecords");

            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_UserId",
                table: "MessageRecords");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Code",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Type",
                table: "Channels");
        }
    }
}
