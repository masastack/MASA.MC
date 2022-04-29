namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class ReceiverItemDto
{
    public string SubjectId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public MessageTaskReceiverType Type { get; set; }
}
