namespace MASA.MC.Service.Admin.Application.Channels.Commands;

public record UpdateChannelCommand(Guid ChannelId, ChannelCreateUpdateDto Channel) : Command
{
    
}
