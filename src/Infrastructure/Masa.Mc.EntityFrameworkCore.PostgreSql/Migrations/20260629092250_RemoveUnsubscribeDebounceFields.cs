using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RemoveUnsubscribeDebounceFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooldownSeconds",
                table: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.DropColumn(
                name: "DebounceEnabled",
                table: "MessageTemplateUnsubscribeConfigs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CooldownSeconds",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DebounceEnabled",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
