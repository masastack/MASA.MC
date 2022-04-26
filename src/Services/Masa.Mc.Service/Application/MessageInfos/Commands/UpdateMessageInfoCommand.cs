namespace Masa.Mc.Service.Admin.Application.MessageInfos.Commands;

public record UpdateMessageInfoCommand(Guid MessageInfoId, MessageInfoCreateUpdateDto MessageInfo) : Command
{
}
