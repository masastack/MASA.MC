namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public record CreateChannelCommand(ChannelCreateUpdateDto Channel) : Command
{
}
