namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Commands;

public record UpdateMessageTemplateCommand(Guid MessageTemplateId, MessageTemplateCreateUpdateDto MessageTemplate) : Command
{
}
