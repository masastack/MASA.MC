// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class ISoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageTasks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageInfos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Channels");
        }
    }
}
