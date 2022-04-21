namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record SmsTemplateSynchroDomainEvent(Guid ChannelId) : DomainEvent
{

}