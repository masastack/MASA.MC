namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public class McDbContext : IntegrationEventLogContext
{
    public DbSet<Channel> Channels { get; set; } = default!;
    public DbSet<MessageTemplate> MessageTemplates { get; set; } = default!;

    public DbSet<ReceiverGroup> ReceiverGroups { get; set; } = default!;

    public McDbContext(MasaDbContextOptions<McDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);
        builder.ConfigureMC();
    }
}
