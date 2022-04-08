namespace MASA.MC.Service.Admin.Application.NotificationTemplates.Profiles;

public class NotificationTemplateProfile : Profile
{
    public NotificationTemplateProfile()
    {
        CreateMap<NotificationTemplate, NotificationTemplateDto>();
        CreateMap<NotificationTemplateCreateUpdateDto, NotificationTemplate>();
    }
}
