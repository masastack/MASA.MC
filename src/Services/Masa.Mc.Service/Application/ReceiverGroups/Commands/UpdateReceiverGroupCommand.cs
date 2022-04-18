namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Commands;

public record UpdateReceiverGroupCommand(Guid ReceiverGroupId, ReceiverGroupCreateUpdateDto ReceiverGroup) : Command
{
}
