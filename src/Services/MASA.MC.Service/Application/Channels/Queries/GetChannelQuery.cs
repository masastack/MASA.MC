using MASA.MC.Contracts.Admin.Dtos.Channels;

namespace MASA.MC.Service.Admin.Application.Channels.Queries;

public record GetChannelQuery(Guid ChannelId) : Query<ChannelDto>
{
    public override ChannelDto Result { get; set; } = new();
}
