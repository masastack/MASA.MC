namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record CreateOrdinaryMessageTaskCommand(MessageTaskCreateUpdateDto MessageTask) : Command
{
}
