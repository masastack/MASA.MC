using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASA.MC.Service.Admin.Migrations
{
    public partial class NotificationTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MCChannels",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MCChannels",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MCNotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Example = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCNotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MCNotificationTemplateItems",
                columns: table => new
                {
                    NotificationTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MappingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCNotificationTemplateItems", x => new { x.Code, x.NotificationTemplateId });
                    table.ForeignKey(
                        name: "FK_MCNotificationTemplateItems_MCNotificationTemplates_NotificationTemplateId",
                        column: x => x.NotificationTemplateId,
                        principalTable: "MCNotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MCNotificationTemplateItems_Code_NotificationTemplateId",
                table: "MCNotificationTemplateItems",
                columns: new[] { "Code", "NotificationTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_MCNotificationTemplateItems_NotificationTemplateId",
                table: "MCNotificationTemplateItems",
                column: "NotificationTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MCNotificationTemplateItems");

            migrationBuilder.DropTable(
                name: "MCNotificationTemplates");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "MCChannels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MCChannels");
        }
    }
}
