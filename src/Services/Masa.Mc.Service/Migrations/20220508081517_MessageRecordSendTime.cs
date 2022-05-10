using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageRecordSendTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletionTime",
                table: "MessageRecords",
                newName: "SendTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SendTime",
                table: "MessageRecords",
                newName: "CompletionTime");
        }
    }
}
