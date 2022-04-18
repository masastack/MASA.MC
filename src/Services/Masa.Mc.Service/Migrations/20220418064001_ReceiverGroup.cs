using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class ReceiverGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiverGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiverGroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiverGroupUsers_ReceiverGroups_ReceiverGroupId",
                        column: x => x.ReceiverGroupId,
                        principalTable: "ReceiverGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupUsers_GroupId_UserId",
                table: "ReceiverGroupUsers",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupUsers_ReceiverGroupId",
                table: "ReceiverGroupUsers",
                column: "ReceiverGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiverGroupUsers");

            migrationBuilder.DropTable(
                name: "ReceiverGroups");
        }
    }
}
