﻿namespace MASA.MC.Admin.Pages.Channels.Modules;

public partial class ChannelSiteExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    private ChannelSiteOptions _model = new();
    private List<string> _environmentItems = new List<string> {"Staging"};
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _model = ExtensionPropertyHelper.ExtraPropertyMapToObj<ChannelSiteOptions>(Value);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task HandleChangeAsync()
    {
        Value = ExtensionPropertyHelper.ObjMapToExtraProperty(_model);
        await ValueChanged.InvokeAsync(Value);
    }
}
