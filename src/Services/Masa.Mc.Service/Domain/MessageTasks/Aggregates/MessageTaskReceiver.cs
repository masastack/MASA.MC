namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskReceiver
{
    public Guid GroupId { get; protected set; }

    public string DataId { get; protected set; }

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    public string PhoneNumber { get; protected set; } = string.Empty;

    public string Email { get; protected set; } = string.Empty;

    public MessageTaskReceiverType Type { get; protected set; }
}
