using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RefactorUnsubscriptionIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnsubscriptionTimelines_OccurredAt",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelId",
                table: "Unsubscriptions");

            // The legacy migration created this index with a shorter truncated name ("...ScopeT~")
            // while newer EF name generation may produce "...ScopeType_Sco~". Drop both variants safely.
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeType_Sco~\";");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeT~\";");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelType",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_Status",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SmsInbounds_ChannelId",
                table: "SmsInbounds");

            migrationBuilder.DropIndex(
                name: "IX_SmsInbounds_Mobile",
                table: "SmsInbounds");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_OccurredAt_Action_UnsubscriptionId",
                table: "UnsubscriptionTimelines",
                columns: new[] { "OccurredAt", "Action", "UnsubscriptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId_OccurredAt",
                table: "UnsubscriptionTimelines",
                columns: new[] { "UnsubscriptionId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeType_Sco~",
                table: "Unsubscriptions",
                columns: new[] { "ChannelId", "ChannelUserIdentity", "ScopeType", "ScopeRefId", "Status", "UnsubscribedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_Status_Unsubs~",
                table: "Unsubscriptions",
                columns: new[] { "ChannelId", "ChannelUserIdentity", "Status", "UnsubscribedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelType_ChannelProvider_UnsubscribedAt",
                table: "Unsubscriptions",
                columns: new[] { "ChannelType", "ChannelProvider", "UnsubscribedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelUserIdentity_Status_ScopeType",
                table: "Unsubscriptions",
                columns: new[] { "ChannelUserIdentity", "Status", "ScopeType" });

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_LastInboundMessageId",
                table: "Unsubscriptions",
                column: "LastInboundMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_ChannelId_SendTime",
                table: "SmsInbounds",
                columns: new[] { "ChannelId", "SendTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Mobile_SendTime",
                table: "SmsInbounds",
                columns: new[] { "Mobile", "SendTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnsubscriptionTimelines_OccurredAt_Action_UnsubscriptionId",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId_OccurredAt",
                table: "UnsubscriptionTimelines");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeType_Sco~",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_Status_Unsubs~",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelType_ChannelProvider_UnsubscribedAt",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_ChannelUserIdentity_Status_ScopeType",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Unsubscriptions_LastInboundMessageId",
                table: "Unsubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SmsInbounds_ChannelId_SendTime",
                table: "SmsInbounds");

            migrationBuilder.DropIndex(
                name: "IX_SmsInbounds_Mobile_SendTime",
                table: "SmsInbounds");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_OccurredAt",
                table: "UnsubscriptionTimelines",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId",
                table: "UnsubscriptionTimelines",
                column: "UnsubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId",
                table: "Unsubscriptions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Unsubscriptions_ChannelId_ChannelUserIdentity_ScopeType_Sco~",
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
                name: "IX_SmsInbounds_ChannelId",
                table: "SmsInbounds",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsInbounds_Mobile",
                table: "SmsInbounds",
                column: "Mobile");
        }
    }
}
