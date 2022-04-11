using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASA.MC.Service.Admin.Migrations
{
    public partial class UpdateMessageTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuditStatus",
                table: "MCMessageTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsJump",
                table: "MCMessageTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JumpUrl",
                table: "MCMessageTemplates",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sign",
                table: "MCMessageTemplates",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditStatus",
                table: "MCMessageTemplates");

            migrationBuilder.DropColumn(
                name: "IsJump",
                table: "MCMessageTemplates");

            migrationBuilder.DropColumn(
                name: "JumpUrl",
                table: "MCMessageTemplates");

            migrationBuilder.DropColumn(
                name: "Sign",
                table: "MCMessageTemplates");
        }
    }
}
