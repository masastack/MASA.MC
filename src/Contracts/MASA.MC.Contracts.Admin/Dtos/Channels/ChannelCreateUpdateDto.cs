using MASA.MC.Contracts.Admin.Enums.Channels;

namespace MASA.MC.Contracts.Admin.Dtos.Channels;

public class ChannelCreateUpdateDto
{
    public string DisplayName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelType Type { get; set; }

    public bool IsStatic { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }=new();

}
