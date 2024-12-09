using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageReceiverUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "MessageRecords");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "MessageReceiverUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MessageReceiverUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MessageReceiverUsers");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "MessageReceiverUsers",
                newName: "ChannelUserIdentity");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "MessageTemplates",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecords_Channels_ChannelId",
                table: "MessageRecords",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecords_Channels_ChannelId",
                table: "MessageRecords");

            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords");

            migrationBuilder.RenameColumn(
                name: "ChannelUserIdentity",
                table: "MessageReceiverUsers",
                newName: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "ChannelType",
                table: "MessageRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "MessageReceiverUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MessageReceiverUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MessageReceiverUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
