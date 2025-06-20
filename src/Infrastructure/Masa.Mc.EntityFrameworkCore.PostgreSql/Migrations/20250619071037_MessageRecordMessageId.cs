using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class MessageRecordMessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "MessageRecords",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_MessageId",
                table: "MessageRecords",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageRecords_MessageId",
                table: "MessageRecords");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "MessageRecords");
        }
    }
}
