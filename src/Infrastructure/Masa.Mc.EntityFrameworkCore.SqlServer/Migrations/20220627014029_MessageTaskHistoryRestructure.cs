using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageTaskHistoryRestructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverType",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "Receivers",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "SelectReceiverType",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "SendRules",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "Sign",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "Variables",
                table: "MessageTaskHistorys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiverType",
                table: "MessageTaskHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Receivers",
                table: "MessageTaskHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SelectReceiverType",
                table: "MessageTaskHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SendRules",
                table: "MessageTaskHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sign",
                table: "MessageTaskHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Variables",
                table: "MessageTaskHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
