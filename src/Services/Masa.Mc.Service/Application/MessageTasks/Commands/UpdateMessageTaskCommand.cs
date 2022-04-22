namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record UpdateMessageTaskCommand(Guid MessageTaskId, MessageTaskCreateUpdateDto MessageTask) : Command
{
}
