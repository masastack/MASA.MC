namespace MASA.MC.Service.Admin.Infrastructure.EntityFrameworkCore;

public class MCDbContext : IntegrationEventLogContext
{
    public DbSet<Channel> Channels { get; set; } = default!;
    public DbSet<MessageTemplate> MessageTemplates { get; set; } = default!;

    public MCDbContext(MasaDbContextOptions<MCDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);
        builder.ConfigureMC();
    }
}
