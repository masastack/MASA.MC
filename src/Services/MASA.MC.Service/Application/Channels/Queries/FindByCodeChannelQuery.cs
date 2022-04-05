using MASA.MC.Contracts.Admin.Dtos.Channels;

namespace MASA.MC.Service.Admin.Application.Channels.Queries;

public record FindByCodeChannelQuery(string Code) : Query<ChannelDto>
{
    public override ChannelDto Result { get; set; } = new();
}
