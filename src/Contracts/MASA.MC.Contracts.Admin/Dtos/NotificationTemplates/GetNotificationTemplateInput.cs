namespace MASA.MC.Contracts.Admin.Dtos.NotificationTemplates;

public class GetNotificationTemplateInput : PaginatedOptionsDto
{
    public GetNotificationTemplateInput(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
