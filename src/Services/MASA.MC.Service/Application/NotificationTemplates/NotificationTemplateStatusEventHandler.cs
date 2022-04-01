using MASA.MC.Contracts.Admin.Dtos.NotificationTemplates;
using MASA.MC.Contracts.Admin.Enums.NotificationTemplates;
using MASA.MC.Service.Admin.Application.NotificationTemplates.Commands;
using MASA.MC.Service.Admin.Domain.NotificationTemplates.Events;

namespace MASA.MC.Service.Admin.Application.NotificationTemplates
{
    public class NotificationTemplateStatusEventHandler
    {
        private readonly IEventBus _eventBus;

        public NotificationTemplateStatusEventHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [EventHandler]
        public async Task NotificationTemplateStatusChangedToApproved(NotificationTemplateStatusChangedToApprovedEvent integrationEvent)
        {
            await _eventBus.PublishAsync(new UpdateNotificationTemplateCommand(integrationEvent.TemplateId,new NotificationTemplateCreateUpdateDto
            {
                Status= NotificationTemplateStatus.Approved
            }));
        }

        [EventHandler]
        public async Task NotificationTemplateStatusChangedToRefuse(NotificationTemplateStatusChangedToRefuseEvent integrationEvent)
        {
            await _eventBus.PublishAsync(new UpdateNotificationTemplateCommand(integrationEvent.TemplateId, new NotificationTemplateCreateUpdateDto
            {
                Status = NotificationTemplateStatus.Refuse
            }));
        }
    }
}
