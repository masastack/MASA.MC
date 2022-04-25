namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskCreateUpdateDto
{
    public Guid ChannelId { get; set; }

    public MessageEntityType EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsEnabled { get; set; }

    public ReceiverType ReceiverType { get; set; }

    public DateTime? SendTime { get; set; }

    //public ExtraPropertyDictionary Receivers { get; set; } = new();

    //public ExtraPropertyDictionary SendingRules { get; set; } = new();

    public ReceiverDto Receivers { get; set; } = new();

    public SendingRuleDto SendingRules { get; set; } = new();
}
