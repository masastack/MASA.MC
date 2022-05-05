using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class SendRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendingRules",
                table: "MessageTasks",
                newName: "SendRules");

            migrationBuilder.RenameColumn(
                name: "SendingRules",
                table: "MessageTaskHistorys",
                newName: "SendRules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendRules",
                table: "MessageTasks",
                newName: "SendingRules");

            migrationBuilder.RenameColumn(
                name: "SendRules",
                table: "MessageTaskHistorys",
                newName: "SendingRules");
        }
    }
}
