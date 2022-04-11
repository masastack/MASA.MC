namespace MASA.MC.Service.Admin.Application.Channels.Queries;

public record FindChannelByCodeQuery(string Code) : Query<ChannelDto>
{
    public override ChannelDto Result { get; set; } = new();
}
