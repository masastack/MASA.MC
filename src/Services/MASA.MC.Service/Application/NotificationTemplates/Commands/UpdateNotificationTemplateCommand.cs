using MASA.MC.Contracts.Admin.Dtos.NotificationTemplates;

namespace MASA.MC.Service.Admin.Application.NotificationTemplates.Commands;

public record UpdateNotificationTemplateCommand(Guid NotificationTemplateId, NotificationTemplateCreateUpdateDto Template) : Command
{
}
