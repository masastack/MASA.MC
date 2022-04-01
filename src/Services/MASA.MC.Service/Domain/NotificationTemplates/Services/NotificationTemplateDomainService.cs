using MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Repositories;

namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Services
{
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
                throw new UserFriendlyException("该模板不允许删除");
            }

            return await _repository.RemoveAsync(template);
        }

        public virtual async Task CreateAsync(NotificationTemplate template)
        {
            await ValidateNotificationTemplateAsync(template.Code);
            await _repository.AddAsync(template);
        }

        public virtual async Task UpdateAsync(NotificationTemplate template)
        {
            await ValidateNotificationTemplateAsync(template.Code, template.Id);
            await _repository.UpdateAsync(template);
        }
        
        protected virtual async Task ValidateNotificationTemplateAsync(string code, Guid? expectedId = null)
        {
            var dict = await _repository.FindAsync(d => d.Code == code);
            if (dict != null && dict.Id != expectedId)
            {
                throw new UserFriendlyException("Template code cannot be repeated");
            }
        }
    }
}
