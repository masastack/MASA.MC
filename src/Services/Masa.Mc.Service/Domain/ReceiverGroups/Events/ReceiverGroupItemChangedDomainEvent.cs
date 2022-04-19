namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Events;

public record ReceiverGroupItemChangedDomainEvent(Guid ReceiverGroupId) : DomainEvent
{
}
