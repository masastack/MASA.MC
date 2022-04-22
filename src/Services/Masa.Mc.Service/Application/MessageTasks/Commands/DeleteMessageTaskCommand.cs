namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record DeleteMessageTaskCommand(Guid MessageTaskId) : Command
{
}
