using MASA.MC.Contracts.Admin.Dtos.Channels;

namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public record CreateChannelCommand(ChannelCreateUpdateDto Channel) : Command
{
}
