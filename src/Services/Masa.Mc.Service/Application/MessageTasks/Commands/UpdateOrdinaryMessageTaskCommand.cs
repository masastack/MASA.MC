namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record UpdateOrdinaryMessageTaskCommand(Guid MessageTaskId, MessageTaskCreateUpdateDto MessageTask) : Command
{
}
