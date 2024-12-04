using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Color = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    Scheme = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SchemeField = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeName = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TimesSent = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ExpandContent = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Markdown = table.Column<string>(type: "text", nullable: false),
                    IsJump = table.Column<bool>(type: "boolean", maxLength: 128, nullable: false),
                    JumpUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Markdown = table.Column<string>(type: "text", nullable: false),
                    IsJump = table.Column<bool>(type: "boolean", maxLength: 128, nullable: false),
                    JumpUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Example = table.Column<string>(type: "text", nullable: false),
                    TemplateId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Sign = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AuditStatus = table.Column<int>(type: "integer", nullable: false),
                    AuditTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InvalidTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AuditReason = table.Column<string>(type: "text", nullable: false),
                    TemplateType = table.Column<int>(type: "integer", nullable: false),
                    PerDayLimit = table.Column<long>(type: "bigint", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiverGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TemplateName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TemplateType = table.Column<int>(type: "integer", nullable: false),
                    AuditStatus = table.Column<int>(type: "integer", nullable: false),
                    TemplateContent = table.Column<string>(type: "text", nullable: false),
                    AuditReason = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteMessageCursors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteMessageCursors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: true),
                    SendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectSendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    MessageEntityType = table.Column<int>(type: "integer", nullable: false),
                    MessageEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelUserIdentity = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SystemId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ChannelType = table.Column<int>(type: "integer", nullable: true),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiverType = table.Column<int>(type: "integer", nullable: false),
                    SelectReceiverType = table.Column<int>(type: "integer", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectSendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Sign = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Receivers = table.Column<string>(type: "text", nullable: false),
                    SendRules = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    SchedulerJobId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTasks_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WebsiteMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    LinkUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsWithdrawn = table.Column<bool>(type: "boolean", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MappingCode = table.Column<string>(type: "text", nullable: false),
                    DisplayText = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Avatar = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverGroupItems", x => new { x.GroupId, x.Id });
                    table.ForeignKey(
                        name: "FK_ReceiverGroupItems_ReceiverGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ReceiverGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTaskHistorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskHistoryNo = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    WithdrawTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsTest = table.Column<bool>(type: "boolean", nullable: false),
                    SchedulerTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "WebsiteMessageTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WebsiteMessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteMessageTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteMessageTags_WebsiteMessages_WebsiteMessageId",
                        column: x => x.WebsiteMessageId,
                        principalTable: "WebsiteMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReceiverUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelUserIdentity = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReceiverUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReceiverUsers_MessageTaskHistorys_MessageTaskHistory~",
                        column: x => x.MessageTaskHistoryId,
                        principalTable: "MessageTaskHistorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Code",
                table: "Channels",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Type",
                table: "Channels",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_EventId_Version",
                table: "IntegrationEventLog",
                columns: new[] { "EventId", "RowVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_State_MTime",
                table: "IntegrationEventLog",
                columns: new[] { "State", "ModificationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_State_TimesSent_MTime",
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
                name: "IX_MessageRecords_MessageTaskHistoryId",
                table: "MessageRecords",
                column: "MessageTaskHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_UserId",
                table: "MessageRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTaskHistorys_MessageTaskId",
                table: "MessageTaskHistorys",
                column: "MessageTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_ChannelId",
                table: "MessageTasks",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTasks_Source",
                table: "MessageTasks",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplateItems_Code_MessageTemplateId",
                table: "MessageTemplateItems",
                columns: new[] { "Code", "MessageTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplateItems_MessageTemplateId",
                table: "MessageTemplateItems",
                column: "MessageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_ChannelId",
                table: "MessageTemplates",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_Code",
                table: "MessageTemplates",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_ChannelId",
                table: "SmsTemplates",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageCursors_UserId",
                table: "WebsiteMessageCursors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessages_ChannelId",
                table: "WebsiteMessages",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessages_UserId_ChannelId",
                table: "WebsiteMessages",
                columns: new[] { "UserId", "ChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_ChannelId",
                table: "WebsiteMessageTags",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_Tag",
                table: "WebsiteMessageTags",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_UserId",
                table: "WebsiteMessageTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteMessageTags_WebsiteMessageId",
                table: "WebsiteMessageTags",
                column: "WebsiteMessageId");
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
                name: "SmsTemplates");

            migrationBuilder.DropTable(
                name: "WebsiteMessageCursors");

            migrationBuilder.DropTable(
                name: "WebsiteMessageTags");

            migrationBuilder.DropTable(
                name: "MessageTaskHistorys");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "ReceiverGroups");

            migrationBuilder.DropTable(
                name: "WebsiteMessages");

            migrationBuilder.DropTable(
                name: "MessageTasks");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
