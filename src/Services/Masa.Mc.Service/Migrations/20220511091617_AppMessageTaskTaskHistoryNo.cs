using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class AppMessageTaskTaskHistoryNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTaskId",
                table: "MessageRecords");

            migrationBuilder.AddColumn<string>(
                name: "TaskHistoryNo",
                table: "MessageTaskHistorys",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskHistoryNo",
                table: "MessageTaskHistorys");

            migrationBuilder.AddColumn<Guid>(
                name: "MessageTaskId",
                table: "MessageRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
