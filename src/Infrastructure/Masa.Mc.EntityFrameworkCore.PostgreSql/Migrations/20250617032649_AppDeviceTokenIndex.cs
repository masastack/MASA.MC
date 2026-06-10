using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AppDeviceTokenIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId",
                table: "AppDeviceTokens");

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

            migrationBuilder.CreateIndex(
                name: "IX_AppDeviceTokens_ChannelId_UserId",
                table: "AppDeviceTokens",
                columns: new[] { "ChannelId", "UserId" });
        }
    }
}
