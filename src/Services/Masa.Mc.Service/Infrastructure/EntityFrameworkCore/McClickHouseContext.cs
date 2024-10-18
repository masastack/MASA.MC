// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public class McClickHouseContext : DbContext
{
    public DbSet<MessageRecordQueryModel> MessageRecords { get; set; }

    public DbSet<WebsiteMessageQueryModel> WebsiteMessages { get; set; }

    public DbSet<WebsiteMessageTagQueryModel> WebsiteMessageTags { get; set; }

    public McClickHouseContext(DbContextOptions<McClickHouseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MessageRecordQueryModel>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(MCConsts.DbTablePrefix + "MessageRecords", MCConsts.DbSchema);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.Property(x => x.Variables).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasIndex(x => x.UserId).IsUnique(false).HasDatabaseName("IX_MessageRecords_UserId").HasMethod("BTREE");
            b.HasIndex(x => x.MessageTaskHistoryId).IsUnique(false).HasDatabaseName("IX_MessageRecords_MessageTaskHistoryId").HasMethod("BTREE");
            b.HasIndex(x => x.CreationTime).IsUnique(false).HasDatabaseName("IX_MessageRecords_CreationTime").HasMethod("BTREE");
            b.Ignore(x => x.Channel);
            b.Ignore(x => x.MessageTask);
        });

        builder.Entity<WebsiteMessageQueryModel>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessages", MCConsts.DbSchema);
            b.Property(x => x.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter()).Metadata.SetValueComparer(new ExtraPropertyDictionaryValueComparer());
            b.HasMany(x => x.Tags).WithOne().HasForeignKey(x => x.WebsiteMessageId).IsRequired();
            b.HasIndex(x => new { x.UserId, x.ChannelId }).IsUnique(false).HasDatabaseName("IX_WebsiteMessages_UserId_ChannelId").HasMethod("BTREE");
            b.HasIndex(x => x.CreationTime).IsUnique(false).HasDatabaseName("IX_WebsiteMessages_CreationTime").HasMethod("BTREE");
            b.Ignore(x => x.Channel);
            b.Ignore(x => x.Tags);
        });

        builder.Entity<WebsiteMessageTagQueryModel>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(MCConsts.DbTablePrefix + "WebsiteMessageTags", MCConsts.DbSchema, table => table
                    .HasMergeTreeEngine()
                    .WithPrimaryKey("Id"));
            b.HasIndex(x => x.Tag).IsUnique(false).HasDatabaseName("IX_WebsiteMessageTags_Tag").HasMethod("BTREE");
            b.HasIndex(x => x.UserId).IsUnique(false).HasDatabaseName("IX_WebsiteMessageTags_UserId").HasMethod("BTREE");
            b.HasIndex(x => x.ChannelId).IsUnique(false).HasDatabaseName("IX_WebsiteMessageTags_ChannelId").HasMethod("BTREE"); ;
            b.HasIndex(x => x.CreationTime).IsUnique(false).HasDatabaseName("IX_WebsiteMessageTags_CreationTime").HasMethod("BTREE");

        });
    }

}