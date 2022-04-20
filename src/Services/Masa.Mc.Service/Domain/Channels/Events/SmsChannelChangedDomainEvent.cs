namespace Masa.Mc.Service.Admin.Domain.Channels.Events;

public record SmsChannelChangedDomainEvent(Guid ChannelId) : DomainEvent
{
}
