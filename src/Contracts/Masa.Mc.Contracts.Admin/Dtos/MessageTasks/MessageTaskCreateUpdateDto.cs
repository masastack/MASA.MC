namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskCreateUpdateDto
{
    public Guid ChannelId { get; set; }

    public MessageEntityType EntityType { get; set; }

    public Guid EntityId { get; set; }

    public ExtraPropertyDictionary Receivers { get; set; } = new();

    public ExtraPropertyDictionary SendingRules { get; set; } = new();

    public bool IsEnabled { get; set; }
}
