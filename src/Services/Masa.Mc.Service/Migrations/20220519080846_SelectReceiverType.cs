using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class SelectReceiverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverSelectType",
                table: "MessageTasks",
                newName: "SelectReceiverType");

            migrationBuilder.RenameColumn(
                name: "ReceiverSelectType",
                table: "MessageTaskHistorys",
                newName: "SelectReceiverType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SelectReceiverType",
                table: "MessageTasks",
                newName: "ReceiverSelectType");

            migrationBuilder.RenameColumn(
                name: "SelectReceiverType",
                table: "MessageTaskHistorys",
                newName: "ReceiverSelectType");
        }
    }
}
