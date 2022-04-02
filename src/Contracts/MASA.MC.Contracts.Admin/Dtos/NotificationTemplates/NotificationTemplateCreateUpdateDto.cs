using MASA.MC.Contracts.Admin.Enums.NotificationTemplates;

namespace MASA.MC.Contracts.Admin.Dtos.NotificationTemplates;

public class NotificationTemplateCreateUpdateDto
{
    public virtual NotificationTemplateStatus Status { get; set; }
}
