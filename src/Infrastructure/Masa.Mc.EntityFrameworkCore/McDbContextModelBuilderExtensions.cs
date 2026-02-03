// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore;

public static class McDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {

        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.Color).HasMaxLength(128).HasColumnName(nameof(Channel.Color));
            b.Property(x => x.Code).IsRequired().HasMaxLength(64).HasColumnName(nameof(Channel.Code));
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128).HasColumnName(nameof(Channel.DisplayName));
            b.Property(x => x.Type).HasColumnName(nameof(Channel.Type));
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.Type).HasConversion(x => x.Id, x => Enumeration.FromValue<ChannelType>(x)).HasColumnName(nameof(Channel.Type));
            b.Property(x => x.Scheme).HasMaxLength(128).HasColumnName(nameof(Channel.Scheme));
            b.Property(x => x.SchemeField).HasMaxLength(128).HasColumnName(nameof(Channel.SchemeField));
            b.Property(x => x.Provider).HasColumnName(nameof(Channel.Provider));
            b.HasIndex(x => x.Code);
            b.HasIndex(x => x.Type);
        });

        builder.Entity<MessageTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplates", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.TemplateId).IsRequired().HasMaxLength(128);
            b.Property(x => x.Sign).HasMaxLength(128);
            b.Property(x => x.Options).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.MessageTemplateId).IsRequired();
            b.HasIndex(x => x.ChannelId);
            b.HasIndex(x => x.Code);
            b.OwnsOne(x => x.MessageContent, b =>
            {
                b.Property(x => x.Title).HasMaxLength(128).HasColumnName("Title");
                b.Property(x => x.Content).HasColumnName("Content");
                b.Property(x => x.Markdown).HasColumnName("Markdown");
                b.Property(x => x.IsJump).HasMaxLength(128).HasColumnName("IsJump");
                b.Property(x => x.JumpUrl).HasMaxLength(256).HasColumnName("JumpUrl");
                b.Property(x => x.ExtraProperties).HasColumnName("ExtraProperties").HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            });
        });

        builder.Entity<MessageTemplateItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplateItems", MCConsts.DbSchema);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayText).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.HasIndex(x => new { x.Code, x.MessageTemplateId });
        });

        builder.Entity<SmsTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "SmsTemplates", MCConsts.DbSchema);
            b.Property(x => x.TemplateName).IsRequired().HasMaxLength(128);
            b.Property(x => x.TemplateCode).IsRequired().HasMaxLength(128);
            b.HasIndex(x => x.ChannelId);
        });

        builder.Entity<ReceiverGroup>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroups", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.OwnsMany(x => x.Items, b =>
            {
                b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroupItems", MCConsts.DbSchema);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
                b.Property(x => x.Avatar).HasMaxLength(128);
                b.WithOwner().HasForeignKey(x => x.GroupId);
            });
        });

        builder.Entity<MessageTask>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTasks", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.Sign).HasMaxLength(128);
            b.Property(x => x.Receivers).HasConversion(new ReceiversValueConverter()).Metadata.SetValueComparer(new ReceiversValueComparer());
            b.Property(x => x.SendRules).HasConversion(new JsonValueConverter<MessageTaskSendingRule>());
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.ChannelType).HasConversion(x => x.Id, x => Enumeration.FromValue<ChannelType>(x));
            b.Property(x => x.SystemId).HasMaxLength(128);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex(x => x.Source);
        });

        builder.Entity<MessageTaskHistory>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTaskHistorys", MCConsts.DbSchema);
            b.Property(x => x.TaskHistoryNo).HasMaxLength(128);
        });

        builder.Entity<MessageInfo>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageInfos", MCConsts.DbSchema);
            b.OwnsOne(x => x.MessageContent, b =>
            {
                b.Property(x => x.Title).HasMaxLength(128).HasColumnName("Title");
                b.Property(x => x.Content).HasColumnName("Content");
                b.Property(x => x.Markdown).HasColumnName("Markdown");
                b.Property(x => x.IsJump).HasMaxLength(128).HasColumnName("IsJump");
                b.Property(x => x.JumpUrl).HasMaxLength(256).HasColumnName("JumpUrl");
                b.Property(x => x.ExtraProperties).HasColumnName("ExtraProperties").HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            });
        });

        builder.Entity<MessageRecord>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageRecords", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.ChannelUserIdentity).IsRequired().HasMaxLength(256);
            b.Property(x => x.SystemId).HasMaxLength(128);
            b.Property(x => x.MessageId).HasMaxLength(256);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.MessageTaskHistoryId);
            b.HasIndex(x => x.MessageId);
        });

        builder.Entity<MessageReceiverUser>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageReceiverUsers", MCConsts.DbSchema);
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.HasKey("Id");
            b.Property(x => x.ChannelUserIdentity).IsRequired().HasMaxLength(256);
            b.Property(x => x.Platform).IsRequired().HasMaxLength(128);
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex("MessageTaskHistoryId", nameof(MessageReceiverUser.ChannelUserIdentity), nameof(MessageReceiverUser.Platform));
        });

        builder.Entity<WebsiteMessage>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessages", MCConsts.DbSchema);
            b.Property(x => x.Title).IsRequired().HasMaxLength(128);
            b.Property(x => x.LinkUrl).HasMaxLength(256);
            b.HasIndex(x => new { x.UserId, x.ChannelId });
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.OwnsMany(x => x.Tags, c =>
            {
                c.ToTable(MCConsts.DbTablePrefix + "WebsiteMessageTags", MCConsts.DbSchema);
                c.Property<Guid>("Id").ValueGeneratedOnAdd();
                c.HasKey("Id");
                c.Property(x => x.Tag).HasMaxLength(128).HasColumnName("Tag");
                c.Property<DateTime>("CreationTime").ValueGeneratedOnAdd();
                c.HasIndex(x => x.Tag);
                c.HasIndex(x => x.UserId);
                c.HasIndex(x => x.ChannelId);
            });
        });

        builder.Entity<WebsiteMessageCursor>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessageCursors", MCConsts.DbSchema);
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<AppDeviceToken>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "AppDeviceTokens", MCConsts.DbSchema);
            b.Property(x => x.DeviceToken).HasMaxLength(256);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex(x => new { x.ChannelId, x.UserId, x.Platform });
        });

        builder.Entity<AppVendorConfig>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "AppVendorConfigs", MCConsts.DbSchema);
            b.Property(x => x.Options).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex(x => new { x.ChannelId, x.Vendor });
        });
    }
}
