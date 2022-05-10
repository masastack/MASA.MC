using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageReceiverUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ReceiverGroupItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ReceiverGroupItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "ReceiverGroupItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "MessageReceiverUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReceiverUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReceiverUsers_MessageTaskHistorys_MessageTaskHistoryId",
                        column: x => x.MessageTaskHistoryId,
                        principalTable: "MessageTaskHistorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId",
                table: "MessageReceiverUsers",
                column: "MessageTaskHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageReceiverUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ReceiverGroupItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ReceiverGroupItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "ReceiverGroupItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);
        }
    }
}
