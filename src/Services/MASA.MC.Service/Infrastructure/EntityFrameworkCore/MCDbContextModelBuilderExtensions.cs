using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Consts;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
using Newtonsoft.Json;

namespace MASA.MC.Service.Admin.Infrastructure.EntityFrameworkCore;

public static class MCDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {
        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(c => c.DisplayName).HasMaxLength(128);
            b.Property(c => c.ExtraProperties).HasConversion(
        v => JsonConvert.SerializeObject(v),
        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        });

        builder.Entity<NotificationTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "NotificationTemplates", MCConsts.DbSchema);
            b.Property(c => c.Code).IsRequired().HasMaxLength(128);
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.NotificationTemplateId).IsRequired();
        });

        builder.Entity<NotificationTemplateItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "NotificationTemplateItems", MCConsts.DbSchema);
            b.Property(x => x.Key).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayText).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);

            b.HasIndex(x => new { x.Key, x.NotificationTemplateId });
            b.HasKey(x => new { x.Key, x.NotificationTemplateId });
        });
    }
}
