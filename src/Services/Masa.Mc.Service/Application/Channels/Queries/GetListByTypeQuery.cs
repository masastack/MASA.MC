namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetListByTypeQuery(ChannelType Type) : Query<List<ChannelDto>>
{
    public override List<ChannelDto> Result { get; set; } = new();
}
