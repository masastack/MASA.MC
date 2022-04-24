using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class ModifyMessageTaskAggregateRoot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiverType",
                table: "MessageTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendTime",
                table: "MessageTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MessageTaskHistorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Receivers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendingRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTaskHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTaskHistorys_MessageTasks_MessageTaskId",
                        column: x => x.MessageTaskId,
                        principalTable: "MessageTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTaskHistorys_MessageTaskId",
                table: "MessageTaskHistorys",
                column: "MessageTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageTaskHistorys");

            migrationBuilder.DropColumn(
                name: "ReceiverType",
                table: "MessageTasks");

            migrationBuilder.DropColumn(
                name: "SendTime",
                table: "MessageTasks");
        }
    }
}
