using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class RefactorUnsubscribeAutoReplyTemplateToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResubscribeAutoReply",
                table: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.DropColumn(
                name: "UnsubscribeAutoReply",
                table: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.AddColumn<Guid>(
                name: "ResubscribeAutoReplyTemplateId",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnsubscribeAutoReplyTemplateId",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResubscribeAutoReplyTemplateId",
                table: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.DropColumn(
                name: "UnsubscribeAutoReplyTemplateId",
                table: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.AddColumn<string>(
                name: "ResubscribeAutoReply",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnsubscribeAutoReply",
                table: "MessageTemplateUnsubscribeConfigs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
