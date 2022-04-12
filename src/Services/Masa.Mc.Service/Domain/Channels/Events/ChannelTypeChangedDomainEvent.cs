namespace Masa.Mc.Service.Admin.Domain.Channels.Events;

public record ChannelTypeChangedDomainEvent(Guid ChannelId, ChannelType ChannelType) : DomainEvent
{
}