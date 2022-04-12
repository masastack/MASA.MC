namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record FindByCodeChannelQuery(string Code) : Query<ChannelDto>
{
    public override ChannelDto Result { get; set; } = new();
}
