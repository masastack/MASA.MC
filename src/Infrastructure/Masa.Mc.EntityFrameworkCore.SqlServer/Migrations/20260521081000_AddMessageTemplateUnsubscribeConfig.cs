using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class AddMessageTemplateUnsubscribeConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageTemplateUnsubscribeConfigs",
                columns: table => new
                {
                    MessageTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UnsubscribeKeyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    UnsubscribeAutoReply = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ResubscribeKeyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    ResubscribeAutoReply = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    DebounceEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CooldownSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateUnsubscribeConfigs", x => x.MessageTemplateId);
                    table.ForeignKey(
                        name: "FK_MessageTemplateUnsubscribeConfigs_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageTemplateUnsubscribeConfigs");
        }
    }
}
