using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AddMessageReceiverUserIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_MessageReceiverUsers_MessageTaskHistoryId\";");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId_ChannelUserIdenti~",
                table: "MessageReceiverUsers",
                columns: new[] { "MessageTaskHistoryId", "ChannelUserIdentity", "Platform" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_MessageReceiverUsers_MessageTaskHistoryId_ChannelUserIdenti~\";");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId",
                table: "MessageReceiverUsers",
                column: "MessageTaskHistoryId");
        }
    }
}
