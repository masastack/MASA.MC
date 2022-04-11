namespace Masa.Mc.Service.Admin.Application.Channels.Commands;

public record UpdateChannelCommand(Guid ChannelId, ChannelCreateUpdateDto Channel) : Command
{
    
}
