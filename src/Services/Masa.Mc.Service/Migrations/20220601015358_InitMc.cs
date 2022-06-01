using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class InitMc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TimesSent = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Markdown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsJump = table.Column<bool>(type: "bit", nullable: false),
                    JumpUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Markdown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Example = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsJump = table.Column<bool>(type: "bit", nullable: false),
                    JumpUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Sign = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuditStatus = table.Column<int>(type: "int", nullable: false),
                    AuditTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvalidTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AuditReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    PerDayLimit = table.Column<long>(type: "bigint", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiverGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    AuditStatus = table.Column<int>(type: "int", nullable: false),
                    TemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteMessageCursors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteMessageCursors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: true),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRecords_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverType = table.Column<int>(type: "int", nullable: false),
                    SelectReceiverType = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Sign = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Receivers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTasks_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteMessages_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MappingCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTemplateItems_MessageTemplates_MessageTemplateId",
                        column: x => x.MessageTemplateId,
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiverGroupItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiverGroupItems_ReceiverGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ReceiverGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiverGroupUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiverGroupUsers_ReceiverGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ReceiverGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTaskHistorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskHistoryNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReceiverType = table.Column<int>(type: "int", nullable: false),
                    SelectReceiverType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Receivers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletionTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    WithdrawTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Sign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "MessageReceiverUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReceiverUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReceiverUsers_MessageTaskHistorys_MessageTaskHistoryId",
                        column: x => x.MessageTaskHistoryId,
                        principalTable: "MessageTaskHistorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "index_eventid_version",
                table: "IntegrationEventLog",
                columns: new[] { "EventId", "RowVersion" });

            migrationBuilder.CreateIndex(
                name: "index_state_modificationtime",
                table: "IntegrationEventLog",
                columns: new[] { "State", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "index_state_timessent_modificationtime",
                table: "IntegrationEventLog",
                columns: new[] { "State", "TimesSent", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId",
                table: "MessageReceiverUsers",
                column: "MessageTaskHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTaskHistorys_MessageTaskId",
                table: "MessageTaskHistorys",
                column: "MessageTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_ChannelId",
                table: "MessageTasks",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplateItems_Code_MessageTemplateId",
                table: "MessageTemplateItems",
                columns: new[] { "Code", "MessageTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplateItems_MessageTemplateId",
                table: "MessageTemplateItems",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupItems_GroupId",
                table: "ReceiverGroupItems",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiverGroupUsers_GroupId_UserId",
                table: "ReceiverGroupUsers",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessages_ChannelId",
                table: "WebsiteMessages",
                column: "ChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLog");

            migrationBuilder.DropTable(
                name: "MessageInfos");

            migrationBuilder.DropTable(
                name: "MessageReceiverUsers");

            migrationBuilder.DropTable(
                name: "MessageRecords");

            migrationBuilder.DropTable(
                name: "MessageTemplateItems");

            migrationBuilder.DropTable(
                name: "ReceiverGroupItems");

            migrationBuilder.DropTable(
                name: "ReceiverGroupUsers");

            migrationBuilder.DropTable(
                name: "SmsTemplates");

            migrationBuilder.DropTable(
                name: "WebsiteMessageCursors");

            migrationBuilder.DropTable(
                name: "WebsiteMessages");

            migrationBuilder.DropTable(
                name: "MessageTaskHistorys");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "ReceiverGroups");

            migrationBuilder.DropTable(
                name: "MessageTasks");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
