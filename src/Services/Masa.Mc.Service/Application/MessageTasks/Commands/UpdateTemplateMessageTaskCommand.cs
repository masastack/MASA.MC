namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record UpdateTemplateMessageTaskCommand(Guid MessageTaskId, MessageTaskCreateUpdateDto MessageTask) : Command
{
}
