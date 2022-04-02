using MASA.MC.Contracts.Admin.Dtos.NotificationTemplates;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;

namespace MASA.MC.Service.Admin.Application.NotificationTemplates.Profiles;

public class NotificationTemplateProfile : Profile
{
    public NotificationTemplateProfile()
    {

        CreateMap<NotificationTemplate, NotificationTemplateDto>();
        CreateMap<NotificationTemplateCreateUpdateDto, NotificationTemplate>();
    }
}
