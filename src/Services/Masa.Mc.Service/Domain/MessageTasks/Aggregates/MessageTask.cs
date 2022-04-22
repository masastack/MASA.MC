namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTask : AuditAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityType EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public ExtraPropertyDictionary Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendingRules { get; protected set; } = new();

    public bool IsEnabled { get; protected set; }
}