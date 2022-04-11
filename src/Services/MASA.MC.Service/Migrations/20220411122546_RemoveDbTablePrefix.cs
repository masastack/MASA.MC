using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASA.MC.Service.Admin.Migrations
{
    public partial class RemoveDbTablePrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MCMessageTemplateItems_MCMessageTemplates_MessageTemplateId",
                table: "MCMessageTemplateItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MCMessageTemplates",
                table: "MCMessageTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MCMessageTemplateItems",
                table: "MCMessageTemplateItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MCChannels",
                table: "MCChannels");

            migrationBuilder.RenameTable(
                name: "MCMessageTemplates",
                newName: "MessageTemplates");

            migrationBuilder.RenameTable(
                name: "MCMessageTemplateItems",
                newName: "MessageTemplateItems");

            migrationBuilder.RenameTable(
                name: "MCChannels",
                newName: "Channels");

            migrationBuilder.RenameIndex(
                name: "IX_MCMessageTemplateItems_MessageTemplateId",
                table: "MessageTemplateItems",
                newName: "IX_MessageTemplateItems_MessageTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_MCMessageTemplateItems_Code_MessageTemplateId",
                table: "MessageTemplateItems",
                newName: "IX_MessageTemplateItems_Code_MessageTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageTemplates",
                table: "MessageTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageTemplateItems",
                table: "MessageTemplateItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Channels",
                table: "Channels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplateItems_MessageTemplates_MessageTemplateId",
                table: "MessageTemplateItems",
                column: "MessageTemplateId",
                principalTable: "MessageTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplateItems_MessageTemplates_MessageTemplateId",
                table: "MessageTemplateItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageTemplates",
                table: "MessageTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageTemplateItems",
                table: "MessageTemplateItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Channels",
                table: "Channels");

            migrationBuilder.RenameTable(
                name: "MessageTemplates",
                newName: "MCMessageTemplates");

            migrationBuilder.RenameTable(
                name: "MessageTemplateItems",
                newName: "MCMessageTemplateItems");

            migrationBuilder.RenameTable(
                name: "Channels",
                newName: "MCChannels");

            migrationBuilder.RenameIndex(
                name: "IX_MessageTemplateItems_MessageTemplateId",
                table: "MCMessageTemplateItems",
                newName: "IX_MCMessageTemplateItems_MessageTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageTemplateItems_Code_MessageTemplateId",
                table: "MCMessageTemplateItems",
                newName: "IX_MCMessageTemplateItems_Code_MessageTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MCMessageTemplates",
                table: "MCMessageTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MCMessageTemplateItems",
                table: "MCMessageTemplateItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MCChannels",
                table: "MCChannels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MCMessageTemplateItems_MCMessageTemplates_MessageTemplateId",
                table: "MCMessageTemplateItems",
                column: "MessageTemplateId",
                principalTable: "MCMessageTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
