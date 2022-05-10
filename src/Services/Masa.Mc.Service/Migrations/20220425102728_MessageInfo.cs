// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class MessageInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Variables",
                table: "MessageTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MessageInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsJump = table.Column<bool>(type: "bit", nullable: false),
                    JumpUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageInfos");

            migrationBuilder.DropColumn(
                name: "Variables",
                table: "MessageTasks");
        }
    }
}
