namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public record CreateMessageTemplateCommand(MessageTemplateCreateUpdateDto MessageTemplate) : Command
{
}
