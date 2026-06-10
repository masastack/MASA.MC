using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class AddSmsInbound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmsInbounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mobile = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SmsContent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AddSerial = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsInbounds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_ChannelId",
                table: "SmsInbounds",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Mobile",
                table: "SmsInbounds",
                column: "Mobile");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Provider",
                table: "SmsInbounds",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_SendTime",
                table: "SmsInbounds",
                column: "SendTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsInbounds");
        }
    }
}
