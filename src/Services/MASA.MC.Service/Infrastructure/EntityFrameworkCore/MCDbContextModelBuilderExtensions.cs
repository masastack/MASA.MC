using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Consts;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
using System.Text.Json;

namespace MASA.MC.Service.Admin.Infrastructure.EntityFrameworkCore;

public static class MCDbContextModelBuilderExtensions
{
    public static void ConfigureMC(this ModelBuilder builder)
    {
        builder.Entity<Channel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(c => c.Code).IsRequired().HasMaxLength(64);
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
            b.Property(c => c.Description).HasMaxLength(512);
            b.Property(c => c.ExtraProperties).HasConversion(v => SerializeObject(v),v => DeserializeObject(v));
        });

        builder.Entity<NotificationTemplate>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "NotificationTemplates", MCConsts.DbSchema);
            b.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.NotificationTemplateId).IsRequired();
        });

        builder.Entity<NotificationTemplateItem>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "NotificationTemplateItems", MCConsts.DbSchema);
            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayText).IsRequired().HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);

            b.HasIndex(x => new { x.Code, x.NotificationTemplateId });
            b.HasKey(x => new { x.Code, x.NotificationTemplateId });
        });
    }
    private static string SerializeObject(Dictionary<string, string> extraProperties)
    {
        return JsonSerializer.Serialize(extraProperties);
    }

    private static Dictionary<string, string> DeserializeObject(string extraPropertiesAsJson)
    {
        if (string.IsNullOrEmpty(extraPropertiesAsJson) || extraPropertiesAsJson == "{}")
        {
            return new Dictionary<string, string>();
        }
        return JsonSerializer.Deserialize<Dictionary<string, string>>(extraPropertiesAsJson) ?? new Dictionary<string, string>();
    }
}
