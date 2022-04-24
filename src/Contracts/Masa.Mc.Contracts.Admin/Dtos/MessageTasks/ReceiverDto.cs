namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class ReceiverDto
{
    public Guid GroupId { get; set; }

    public string DataId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public MessageTaskReceiverType Type { get; set; }
}
