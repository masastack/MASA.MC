using MASA.MC.Contracts.Admin.Enums.Channels;

namespace MASA.MC.Contracts.Admin.Dtos.Channels;

public class ChannelDto : AuditEntityDto<Guid, Guid?>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelType Type { get; set; }

    public bool IsStatic { get; set; }

    public Dictionary<string, string> ExtraProperties { get; set; } = new();
}
