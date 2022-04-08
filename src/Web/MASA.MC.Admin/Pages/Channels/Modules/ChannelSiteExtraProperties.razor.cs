namespace MASA.MC.Admin.Pages.Channels.Modules;

public partial class ChannelSiteExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    private ChannelSiteOptions _model = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _model = ExtensionPropertyHelper.ExtraPropertyMapToObj<ChannelSiteOptions>(Value);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public void HandleChangeAsync()
    {
        Value = ExtensionPropertyHelper.ObjMapToExtraProperty(_model);
        ValueChanged.InvokeAsync(Value);
    }
}
