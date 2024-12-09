using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class UpdateWebsiteMessageTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChannelId",
                table: "WebsiteMessageTags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "WebsiteMessageTags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_ChannelId",
                table: "WebsiteMessageTags",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_UserId",
                table: "WebsiteMessageTags",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WebsiteMessageTags_ChannelId",
                table: "WebsiteMessageTags");

            migrationBuilder.DropIndex(
                name: "IX_WebsiteMessageTags_UserId",
                table: "WebsiteMessageTags");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "WebsiteMessageTags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WebsiteMessageTags");
        }
    }
}
