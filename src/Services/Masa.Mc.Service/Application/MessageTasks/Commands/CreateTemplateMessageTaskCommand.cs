namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record CreateTemplateMessageTaskCommand(MessageTaskCreateUpdateDto MessageTask) : Command
{
}
