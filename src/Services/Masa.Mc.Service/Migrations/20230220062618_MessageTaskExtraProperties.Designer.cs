﻿// <auto-generated />
using System;
using Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Masa.Mc.Service.Admin.Migrations
{
    [DbContext(typeof(McDbContext))]
    [Migration("20230220062618_MessageTaskExtraProperties")]
    partial class MessageTaskExtraProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs.IntegrationEventLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)")
                        .HasColumnName("RowVersion");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("TimesSent")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "EventId", "RowVersion" }, "IX_EventId_Version");

                    b.HasIndex(new[] { "State", "ModificationTime" }, "IX_State_MTime");

                    b.HasIndex(new[] { "State", "TimesSent", "ModificationTime" }, "IX_State_TimesSent_MTime");

                    b.ToTable("IntegrationEventLog", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("Code");

                    b.Property<string>("Color")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("Color");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("DisplayName");

                    b.Property<int>("Type")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("int")
                        .HasColumnName("Type");

                    b.HasKey("Id");

                    b.ToTable("Channels", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.Channel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("Code");

                    b.Property<string>("Color")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("Color");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("DisplayName");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStatic")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("int")
                        .HasColumnName("Type");

                    b.HasKey("Id");

                    b.HasIndex("Code");

                    b.HasIndex("Type");

                    b.ToTable("Channels", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates.MessageInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("MessageInfos", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates.MessageRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChannelUserIdentity")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTimeOffset?>("ExpectSendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FailureReason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("MessageEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MessageEntityType")
                        .HasColumnType("int");

                    b.Property<Guid>("MessageTaskHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MessageTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("SendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool?>("Success")
                        .HasColumnType("bit");

                    b.Property<string>("SystemId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Variables")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("MessageTaskHistoryId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageRecords", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageReceiverUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChannelUserIdentity")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid?>("MessageTaskHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Variables")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MessageTaskHistoryId");

                    b.ToTable("MessageReceiverUsers", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ChannelType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EntityType")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("ExpectSendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDraft")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ReceiverType")
                        .HasColumnType("int");

                    b.Property<string>("ReceiverUsers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Receivers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SchedulerJobId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SelectReceiverType")
                        .HasColumnType("int");

                    b.Property<string>("SendRules")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("SendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Sign")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("SystemId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Variables")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("MessageTasks", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTaskHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("CompletionTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTest")
                        .HasColumnType("bit");

                    b.Property<Guid>("MessageTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SchedulerTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("SendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TaskHistoryNo")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTimeOffset?>("WithdrawTime")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("MessageTaskId");

                    b.ToTable("MessageTaskHistorys", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuditReason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AuditStatus")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("AuditTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Example")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("InvalidTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStatic")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("PerDayLimit")
                        .HasColumnType("bigint");

                    b.Property<string>("Sign")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TemplateId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("TemplateType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("Code");

                    b.ToTable("MessageTemplates", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplateItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("DisplayText")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("MappingCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MessageTemplateId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("MessageTemplateId");

                    b.HasIndex("Code", "MessageTemplateId");

                    b.ToTable("MessageTemplateItems", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.SmsTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuditReason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AuditStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TemplateCode")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("TemplateContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("TemplateType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("SmsTemplates", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates.ReceiverGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ReceiverGroups", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates.WebsiteMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWithdrawn")
                        .HasColumnType("bit");

                    b.Property<string>("LinkUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid>("MessageTaskHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("ReadTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("SendTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId", "ChannelId");

                    b.ToTable("WebsiteMessages", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates.WebsiteMessageCursor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("UpdateTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("WebsiteMessageCursors", (string)null);
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.Channel", null)
                        .WithOne()
                        .HasForeignKey("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates.MessageInfo", b =>
                {
                    b.OwnsOne("Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates.MessageContent", "MessageContent", b1 =>
                        {
                            b1.Property<Guid>("MessageInfoId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Content")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Content");

                            b1.Property<string>("ExtraProperties")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("ExtraProperties");

                            b1.Property<bool>("IsJump")
                                .HasMaxLength(128)
                                .HasColumnType("bit")
                                .HasColumnName("IsJump");

                            b1.Property<string>("JumpUrl")
                                .IsRequired()
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)")
                                .HasColumnName("JumpUrl");

                            b1.Property<string>("Markdown")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Markdown");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("nvarchar(128)")
                                .HasColumnName("Title");

                            b1.HasKey("MessageInfoId");

                            b1.ToTable("MessageInfos");

                            b1.WithOwner()
                                .HasForeignKey("MessageInfoId");
                        });

                    b.Navigation("MessageContent")
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates.MessageRecord", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageReceiverUser", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTaskHistory", null)
                        .WithMany("ReceiverUsers")
                        .HasForeignKey("MessageTaskHistoryId");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTask", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTaskHistory", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTask", "MessageTask")
                        .WithMany()
                        .HasForeignKey("MessageTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MessageTask");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplate", b =>
                {
                    b.OwnsOne("Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates.MessageContent", "MessageContent", b1 =>
                        {
                            b1.Property<Guid>("MessageTemplateId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Content")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Content");

                            b1.Property<bool>("IsJump")
                                .HasMaxLength(128)
                                .HasColumnType("bit")
                                .HasColumnName("IsJump");

                            b1.Property<string>("JumpUrl")
                                .IsRequired()
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)")
                                .HasColumnName("JumpUrl");

                            b1.Property<string>("Markdown")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Markdown");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("nvarchar(128)")
                                .HasColumnName("Title");

                            b1.HasKey("MessageTemplateId");

                            b1.ToTable("MessageTemplates");

                            b1.WithOwner()
                                .HasForeignKey("MessageTemplateId");
                        });

                    b.Navigation("MessageContent")
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplateItem", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplate", null)
                        .WithMany("Items")
                        .HasForeignKey("MessageTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates.ReceiverGroup", b =>
                {
                    b.OwnsMany("Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates.ReceiverGroupItem", "Items", b1 =>
                        {
                            b1.Property<Guid>("GroupId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Type")
                                .HasColumnType("int");

                            b1.HasKey("GroupId", "Id");

                            b1.ToTable("ReceiverGroupItems", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("GroupId");

                            b1.OwnsOne("Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates.Receiver", "Receiver", b2 =>
                                {
                                    b2.Property<Guid>("ReceiverGroupItemGroupId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<Guid>("ReceiverGroupItemId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<string>("Avatar")
                                        .IsRequired()
                                        .HasMaxLength(128)
                                        .HasColumnType("nvarchar(128)")
                                        .HasColumnName("Avatar");

                                    b2.Property<string>("DisplayName")
                                        .IsRequired()
                                        .HasMaxLength(128)
                                        .HasColumnType("nvarchar(128)")
                                        .HasColumnName("DisplayName");

                                    b2.Property<string>("Email")
                                        .IsRequired()
                                        .HasMaxLength(128)
                                        .HasColumnType("nvarchar(128)")
                                        .HasColumnName("Email");

                                    b2.Property<string>("PhoneNumber")
                                        .IsRequired()
                                        .HasMaxLength(128)
                                        .HasColumnType("nvarchar(128)")
                                        .HasColumnName("PhoneNumber");

                                    b2.Property<Guid>("SubjectId")
                                        .HasColumnType("uniqueidentifier")
                                        .HasColumnName("SubjectId");

                                    b2.HasKey("ReceiverGroupItemGroupId", "ReceiverGroupItemId");

                                    b2.ToTable("ReceiverGroupItems");

                                    b2.WithOwner()
                                        .HasForeignKey("ReceiverGroupItemGroupId", "ReceiverGroupItemId");
                                });

                            b1.Navigation("Receiver")
                                .IsRequired();
                        });

                    b.Navigation("Items");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates.WebsiteMessage", b =>
                {
                    b.HasOne("Masa.Mc.Service.Admin.Domain.Channels.Aggregates.AppChannel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates.MessageTaskHistory", b =>
                {
                    b.Navigation("ReceiverUsers");
                });

            modelBuilder.Entity("Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates.MessageTemplate", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
