namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class ReceiverGroupCreateUpdateDto
{
    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Guid> UserIds { get; set; } = new();
}