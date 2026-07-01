using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AddMessageRecordHistorySuccessIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId",
                table: "MessageRecords");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId_Success",
                table: "MessageRecords",
                columns: new[] { "MessageTaskHistoryId", "Success" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId_Success",
                table: "MessageRecords");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_MessageTaskHistoryId",
                table: "MessageRecords",
                column: "MessageTaskHistoryId");
        }
    }
}
