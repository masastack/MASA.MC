﻿namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public class McDbContext : IsolationDbContext
{
    public DbSet<Channel> Channels { get; set; } = default!;

    public DbSet<MessageTemplate> MessageTemplates { get; set; } = default!;

    public DbSet<ReceiverGroup> ReceiverGroups { get; set; } = default!;

    public DbSet<SmsTemplate> SmsTemplates { get; set; } = default!;

    public DbSet<MessageTask> MessageTasks { get; set; } = default!;

    public McDbContext(MasaDbContextOptions<McDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);

        builder.Entity<AppChannel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.Code).HasColumnName(nameof(Channel.Code));
            b.Property(x => x.DisplayName).HasColumnName(nameof(Channel.DisplayName));
            b.Property(x => x.Type).HasColumnName(nameof(Channel.Type));
            b.HasOne<Channel>().WithOne().HasForeignKey<AppChannel>(x => x.Id);
        });

        builder.ConfigureMC();
    }
}
