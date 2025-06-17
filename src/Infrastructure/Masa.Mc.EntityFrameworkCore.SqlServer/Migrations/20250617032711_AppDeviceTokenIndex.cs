using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class AppDeviceTokenIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId",
                table: "AppDeviceTokens");

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "MessageReceiverUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId_Platform",
                table: "AppDeviceTokens",
                columns: new[] { "ChannelId", "UserId", "Platform" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId_Platform",
                table: "AppDeviceTokens");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "MessageReceiverUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId",
                table: "AppDeviceTokens",
                columns: new[] { "ChannelId", "UserId" });
        }
    }
}
