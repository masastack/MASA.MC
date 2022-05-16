using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class AppMessageTaskHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppMessageTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMessageTask", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTaskHistorys_AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys",
                column: "AppMessageTaskHistory_MessageTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTaskHistorys_AppMessageTask_AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys",
                column: "AppMessageTaskHistory_MessageTaskId",
                principalTable: "AppMessageTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTaskHistorys_AppMessageTask_AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys");

            migrationBuilder.DropTable(
                name: "AppMessageTask");

            migrationBuilder.DropIndex(
                name: "IX_MessageTaskHistorys_AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "AppMessageTaskHistory_MessageTaskId",
                table: "MessageTaskHistorys");
        }
    }
}
