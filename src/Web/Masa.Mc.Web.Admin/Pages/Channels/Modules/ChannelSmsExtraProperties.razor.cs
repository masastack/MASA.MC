namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelSmsExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    private ChannelSmsOptions _model = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _model = ExtensionPropertyHelper.ExtraPropertyMapToObj<ChannelSmsOptions>(Value);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task HandleChangeAsync()
    {
        Value = ExtensionPropertyHelper.ObjMapToExtraProperty(_model);
        await ValueChanged.InvokeAsync(Value);
    }
}
