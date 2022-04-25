namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskDto : AuditEntityDto<Guid, Guid>
{
    public Guid ChannelId { get; set; }

    public ChannelDto Channel { get; set; }

    public MessageEntityType EntityType { get; set; }

    public Guid EntityId { get; set; }

    public ExtraPropertyDictionary Receivers { get; set; } = new();

    public ExtraPropertyDictionary SendingRules { get; set; } = new();

    public bool IsEnabled { get; set; }

    public DateTime? SendTime { get; set; }

    public MessageInfoDto MessageInfo { get; set; }
}
