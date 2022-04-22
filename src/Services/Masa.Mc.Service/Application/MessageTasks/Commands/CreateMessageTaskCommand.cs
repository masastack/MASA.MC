namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record CreateMessageTaskCommand(MessageTaskCreateUpdateDto MessageTask) : Command
{
}
