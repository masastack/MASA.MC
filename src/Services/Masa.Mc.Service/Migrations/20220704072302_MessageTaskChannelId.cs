using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageTaskChannelId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTasks_Channels_ChannelId",
                table: "MessageTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "MessageTasks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTasks_Channels_ChannelId",
                table: "MessageTasks",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTasks_Channels_ChannelId",
                table: "MessageTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "MessageTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTasks_Channels_ChannelId",
                table: "MessageTasks",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
