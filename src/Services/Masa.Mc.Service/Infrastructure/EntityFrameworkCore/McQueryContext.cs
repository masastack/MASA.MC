﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public class McQueryContext : MasaDbContext<McQueryContext>, IMcQueryContext
{
    public IQueryable<MessageTaskQueryModel> MessageTaskQueries => Set<MessageTaskQueryModel>().AsQueryable();

    public IQueryable<MessageTaskHistoryQueryModel> MessageTaskHistoryQueries => Set<MessageTaskHistoryQueryModel>().AsQueryable();

    public IQueryable<ChannelQueryModel> ChannelQueryQueries => Set<ChannelQueryModel>().AsQueryable();

    public IQueryable<MessageInfoQueryModel> MessageInfoQueries => Set<MessageInfoQueryModel>().AsQueryable();

    public IQueryable<MessageRecordQueryModel> MessageRecordQueries => Set<MessageRecordQueryModel>().AsQueryable();

    public IQueryable<MessageTemplateQueryModel> MessageTemplateQueries => Set<MessageTemplateQueryModel>().AsQueryable();

    public IQueryable<ReceiverGroupQueryModel> ReceiverGroupQueries => Set<ReceiverGroupQueryModel>().AsQueryable();

    public IQueryable<WebsiteMessageQueryModel> WebsiteMessageQueries => Set<WebsiteMessageQueryModel>().AsQueryable();

    public McQueryContext(MasaDbContextOptions<McQueryContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.Entity<ChannelQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<MessageTemplateQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageTemplates", MCConsts.DbSchema);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.MessageTemplateId).IsRequired();
        });

        builder.Entity<MessageTemplateItemQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageTemplateItems", MCConsts.DbSchema);
        });

        builder.Entity<ReceiverGroupQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "ReceiverGroups", MCConsts.DbSchema);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.GroupId).IsRequired();
        });

        builder.Entity<ReceiverGroupItemQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "ReceiverGroupItems", MCConsts.DbSchema);
        });

        builder.Entity<MessageTaskQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageTasks", MCConsts.DbSchema);
            b.Property(x => x.Receivers).HasConversion(new JsonValueConverter<List<MessageTaskReceiver>>());
            b.Property(x => x.SendRules).HasConversion(new JsonValueConverter<MessageTaskSendingRule>());
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<MessageTaskHistoryQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageTaskHistorys", MCConsts.DbSchema);
        });

        builder.Entity<MessageInfoQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageInfos", MCConsts.DbSchema);
        });

        builder.Entity<MessageRecordQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageRecords", MCConsts.DbSchema);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<MessageReceiverUserQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "MessageReceiverUsers", MCConsts.DbSchema);
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<WebsiteMessageQueryModel>(b =>
        {
            b.ToView(MCConsts.DbTablePrefix + "WebsiteMessages", MCConsts.DbSchema);
        });

        base.OnModelCreatingExecuting(builder);
    }
}
