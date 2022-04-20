using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class UpdateReceiverGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiverGroupUsers_ReceiverGroups_ReceiverGroupId",
                table: "ReceiverGroupUsers");

            migrationBuilder.DropIndex(
                name: "IX_ReceiverGroupUsers_ReceiverGroupId",
                table: "ReceiverGroupUsers");

            migrationBuilder.DropColumn(
                name: "ReceiverGroupId",
                table: "ReceiverGroupUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiverGroupUsers_ReceiverGroups_GroupId",
                table: "ReceiverGroupUsers",
                column: "GroupId",
                principalTable: "ReceiverGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiverGroupUsers_ReceiverGroups_GroupId",
                table: "ReceiverGroupUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiverGroupId",
                table: "ReceiverGroupUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupUsers_ReceiverGroupId",
                table: "ReceiverGroupUsers",
                column: "ReceiverGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiverGroupUsers_ReceiverGroups_ReceiverGroupId",
                table: "ReceiverGroupUsers",
                column: "ReceiverGroupId",
                principalTable: "ReceiverGroups",
                principalColumn: "Id");
        }
    }
}
