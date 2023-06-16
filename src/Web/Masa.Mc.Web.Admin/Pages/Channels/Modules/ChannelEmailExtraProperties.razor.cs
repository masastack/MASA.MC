// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelEmailExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    [Parameter]
    public bool PasswordView { get; set; }

    public MForm Form { get; set; } = default!;

    private EmailChannelOptions _model = new();
    private bool _passwordShow;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _model = ExtensionPropertyHelper.ConvertToType<EmailChannelOptions>(Value);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task HandleChangeAsync()
    {
        Value = ExtensionPropertyHelper.ConvertToExtraProperty(_model);
        await ValueChanged.InvokeAsync(Value);
    }
}
