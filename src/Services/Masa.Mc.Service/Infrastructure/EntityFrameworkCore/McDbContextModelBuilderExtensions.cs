namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public static class McDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {
        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(c => c.Code).IsRequired().HasMaxLength(64);
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(c => c.Description).HasMaxLength(512);
            b.Property(c => c.ExtraProperties).HasConversion(new ExtraPropertiesValueConverter());
        });

        builder.Entity<MessageTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "MessageTemplates", MCConsts.DbSchema);
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
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
    }
    
}
