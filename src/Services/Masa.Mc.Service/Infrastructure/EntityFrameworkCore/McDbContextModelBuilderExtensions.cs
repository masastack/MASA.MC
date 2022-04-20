﻿namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public static class McDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {
        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter());
        });

        builder.Entity<MessageTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplates", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(x => x.JumpUrl).HasMaxLength(256);
            b.Property(x => x.Sign).HasMaxLength(128);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.MessageTemplateId).IsRequired();
        });

        builder.Entity<MessageTemplateItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplateItems", MCConsts.DbSchema);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayText).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.HasIndex(x => new { x.Code, x.MessageTemplateId });
        });

        builder.Entity<ReceiverGroup>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroups", MCConsts.DbSchema);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            b.HasMany(x => x.Users).WithOne().HasForeignKey(x => x.GroupId).IsRequired();
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.GroupId).IsRequired();
        });

        builder.Entity<ReceiverGroupUser>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroupUsers", MCConsts.DbSchema);
            b.HasIndex(x => new { x.GroupId, x.UserId });
        });

        builder.Entity<ReceiverGroupItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "ReceiverGroupItems", MCConsts.DbSchema);
        });
    }

}
