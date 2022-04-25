namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class ReceiverGroupDto : AuditEntityDto<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ReceiverGroupUserDto> Users { get; set; } = new();

    public List<ReceiverGroupItemDto> Items { get; set; } = new();

    public string ModifierName { get; set; } = string.Empty;
}