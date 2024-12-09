using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageInfoType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverUsers",
                table: "MessageTasks");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "MessageInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_Source",
                table: "MessageTasks",
                column: "Source");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageTasks_Source",
                table: "MessageTasks");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "MessageInfos");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUsers",
                table: "MessageTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
