using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class InitialBaseline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDeviceTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    RegisteredTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDeviceTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppVendorConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vendor = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVendorConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Scheme = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SchemeField = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
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
                    ExpandContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    IsJump = table.Column<bool>(type: "bit", maxLength: 128, nullable: false),
                    JumpUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
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
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Markdown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsJump = table.Column<bool>(type: "bit", maxLength: 128, nullable: false),
                    JumpUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Example = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Sign = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuditStatus = table.Column<int>(type: "int", nullable: false),
                    AuditTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvalidTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AuditReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    PerDayLimit = table.Column<long>(type: "bigint", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "SmsInbounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SmsContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AddSerial = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsInbounds", x => x.Id);
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
                    ExpectSendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MessageEntityType = table.Column<int>(type: "int", nullable: false),
                    MessageEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelUserIdentity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SystemId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
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
                    ChannelType = table.Column<int>(type: "int", nullable: true),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverType = table.Column<int>(type: "int", nullable: false),
                    SelectReceiverType = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpectSendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Sign = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Receivers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SchedulerJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        principalColumn: "Id");
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
                    LinkUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsWithdrawn = table.Column<bool>(type: "bit", nullable: false),
                    MessageTaskHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "MessageTemplateUnsubscribeConfigs",
                columns: table => new
                {
                    MessageTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UnsubscribeKeyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    UnsubscribeAutoReply = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ResubscribeKeyword = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    ResubscribeAutoReply = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    DebounceEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CooldownSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateUnsubscribeConfigs", x => x.MessageTemplateId);
                    table.ForeignKey(
                        name: "FK_MessageTemplateUnsubscribeConfigs_MessageTemplates_MessageTemplateId",
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
                    Type = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
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
                    MatchedMessageRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MatchedMessageSnapshot = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MatchedMessageSentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "MessageTaskHistorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskHistoryNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletionTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    WithdrawTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    SchedulerTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "WebsiteMessageTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WebsiteMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelUserIdentity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                name: "IX_AppDeviceTokens_ChannelId_UserId_Platform",
                table: "AppDeviceTokens",
                columns: new[] { "ChannelId", "UserId", "Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_AppVendorConfigs_ChannelId_Vendor",
                table: "AppVendorConfigs",
                columns: new[] { "ChannelId", "Vendor" });

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
                name: "IX_MessageReceiverUsers_MessageTaskHistoryId_ChannelUserIdentity_Platform",
                table: "MessageReceiverUsers",
                columns: new[] { "MessageTaskHistoryId", "ChannelUserIdentity", "Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_ChannelId",
                table: "MessageRecords",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_MessageId",
                table: "MessageRecords",
                column: "MessageId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_ChannelId",
                table: "SmsTemplates",
                column: "ChannelId");

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
                name: "IX_UnsubscriptionTimelines_OccurredAt",
                table: "UnsubscriptionTimelines",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscriptionTimelines_UnsubscriptionId",
                table: "UnsubscriptionTimelines",
                column: "UnsubscriptionId");

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
                name: "AppDeviceTokens");

            migrationBuilder.DropTable(
                name: "AppVendorConfigs");

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
                name: "MessageTemplateUnsubscribeConfigs");

            migrationBuilder.DropTable(
                name: "ReceiverGroupItems");

            migrationBuilder.DropTable(
                name: "SmsInbounds");

            migrationBuilder.DropTable(
                name: "SmsTemplates");

            migrationBuilder.DropTable(
                name: "UnsubscriptionTimelines");

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
                name: "Unsubscriptions");

            migrationBuilder.DropTable(
                name: "WebsiteMessages");

            migrationBuilder.DropTable(
                name: "MessageTasks");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
