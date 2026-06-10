using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class AddUnsubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Unsubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelUserIdentity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ChannelType = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelProvider = table.Column<int>(type: "int", nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    ScopeRefId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UnsubscribedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResubscribedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastInboundMessageId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unsubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnsubscriptionTimelines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnsubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnsubscriptionTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnsubscriptionTimelines_Unsubscriptions_UnsubscriptionId",
                        column: x => x.UnsubscriptionId,
                        principalTable: "Unsubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId",
                table: "Unsubscriptions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeType_ScopeRefId_Status",
                table: "Unsubscriptions",
                columns: new[] { "ChannelId", "ChannelUserIdentity", "ScopeType", "ScopeRefId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelType",
                table: "Unsubscriptions",
                column: "ChannelType");

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_Status",
                table: "Unsubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_UserId",
                table: "Unsubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId",
                table: "UnsubscriptionTimelines",
                column: "UnsubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_OccurredAt",
                table: "UnsubscriptionTimelines",
                column: "OccurredAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnsubscriptionTimelines");

            migrationBuilder.DropTable(
                name: "Unsubscriptions");
        }
    }
}
