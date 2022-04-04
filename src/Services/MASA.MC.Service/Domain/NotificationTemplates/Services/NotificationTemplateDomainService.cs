using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Repositories;

namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Services;

public class NotificationTemplateDomainService : DomainService
{
    private readonly INotificationTemplateRepository _repository;

    public NotificationTemplateDomainService(IDomainEventBus eventBus, INotificationTemplateRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public async Task<NotificationTemplate> DeleteAsync(NotificationTemplate template)
    {
        if (template.IsStatic)
        {
            throw new UserFriendlyException("The template cannot be deleted");
        }

        return await _repository.RemoveAsync(template);
    }
}
