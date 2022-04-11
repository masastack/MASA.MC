namespace MASA.MC.Contracts.Admin.Dtos.Channels;

public class ChannelCreateUpdateDto
{
    public string DisplayName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelType Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsStatic { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; }=new();
}
