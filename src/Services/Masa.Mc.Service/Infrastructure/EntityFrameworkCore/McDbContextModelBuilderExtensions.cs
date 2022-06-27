// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public static class McDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {

        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.Color).HasMaxLength(128).HasColumnName(nameof(Channel.Color));
            b.Property(c => c.Code).IsRequired().HasMaxLength(64).HasColumnName(nameof(Channel.Code));
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128).HasColumnName(nameof(Channel.DisplayName));
            b.Property(c => c.Type).HasColumnName(nameof(Channel.Type));
            b.Property(c => c.Description).HasMaxLength(512);
            b.Property(c => c.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<MessageTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplates", MCConsts.DbSchema);
            b.Property(m => m.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(m => m.TemplateId).IsRequired().HasMaxLength(128);
            b.Property(m => m.JumpUrl).HasMaxLength(256);
            b.Property(m => m.Sign).HasMaxLength(128);
            b.HasMany(m => m.Items).WithOne().HasForeignKey(m => m.MessageTemplateId).IsRequired();
        });

        builder.Entity<MessageTemplateItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplateItems", MCConsts.DbSchema);
            b.Property(m => m.Code).IsRequired().HasMaxLength(64);
            b.Property(m => m.DisplayText).IsRequired().HasMaxLength(128);
            b.Property(m => m.Description).HasMaxLength(512);
            b.HasIndex(m => new { m.Code, m.MessageTemplateId });
        });

        builder.Entity<SmsTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "SmsTemplates", MCConsts.DbSchema);
            b.Property(s => s.TemplateName).IsRequired().HasMaxLength(128);
            b.Property(s => s.TemplateCode).IsRequired().HasMaxLength(128);
        });

        builder.Entity<ReceiverGroup>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroups", MCConsts.DbSchema);
            b.Property(r => r.DisplayName).IsRequired().HasMaxLength(128);
            b.HasMany(r => r.Users).WithOne().HasForeignKey(r => r.GroupId).IsRequired();
            b.HasMany(r => r.Items).WithOne().HasForeignKey(r => r.GroupId).IsRequired();
        });

        builder.Entity<ReceiverGroupUser>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroupUsers", MCConsts.DbSchema);
            b.HasIndex(r => new { r.GroupId, r.UserId });
        });

        builder.Entity<ReceiverGroupItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroupItems", MCConsts.DbSchema);
            b.Property(r => r.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(r => r.PhoneNumber).HasMaxLength(128);
            b.Property(r => r.Email).HasMaxLength(128);
        });

        builder.Entity<MessageTask>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTasks", MCConsts.DbSchema);
            b.Property(m => m.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(m => m.Sign).HasMaxLength(128);
            b.Property(m => m.Receivers).HasConversion(new ReceiversValueConverter()).Metadata.SetValueComparer(new ReceiversValueComparer());
            b.Property(m => m.SendRules).HasConversion(new JsonValueConverter<MessageTaskSendingRule>());
            b.Property(m => m.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.ReceiverUsers).HasConversion(new JsonValueConverter<List<MessageReceiverUser>>());
        });

        builder.Entity<MessageTaskHistory>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTaskHistorys", MCConsts.DbSchema);
            b.Property(m => m.TaskHistoryNo).HasMaxLength(128);
        });

        builder.Entity<MessageInfo>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageInfos", MCConsts.DbSchema);
            b.Property(m => m.Title).IsRequired().HasMaxLength(128);
            b.Property(m => m.JumpUrl).HasMaxLength(256);
        });

        builder.Entity<MessageRecord>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageRecords", MCConsts.DbSchema);
            b.Property(m => m.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(m => m.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Ignore(m => m.MessageTask);
        });

        builder.Entity<MessageReceiverUser>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageReceiverUsers", MCConsts.DbSchema);
            b.Property(m => m.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(m => m.PhoneNumber).HasMaxLength(128);
            b.Property(m => m.Email).HasMaxLength(128);
            b.Property(m => m.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
        });

        builder.Entity<WebsiteMessage>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessages", MCConsts.DbSchema);
            b.Property(w => w.Title).IsRequired().HasMaxLength(128);
            b.Property(m => m.LinkUrl).HasMaxLength(256);
        });

        builder.Entity<WebsiteMessageCursor>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessageCursors", MCConsts.DbSchema);
        });
    }
}
