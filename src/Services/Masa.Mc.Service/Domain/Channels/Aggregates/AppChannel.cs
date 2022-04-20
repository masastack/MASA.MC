namespace Masa.Mc.Service.Admin.Domain.Channels.Aggregates;

public class AppChannel : Entity<Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;
    public string Code { get; protected set; } = string.Empty;
    public ChannelType Type { get; protected set; }
}
