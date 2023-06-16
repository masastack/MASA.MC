// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelAppExtraProperties : AdminCompontentBase
{
    [Parameter]
    public ExtraPropertyDictionary Value { get; set; } = new();

    [Parameter]
    public EventCallback<ExtraPropertyDictionary> ValueChanged { get; set; }

    [Parameter]
    public bool PasswordView { get; set; }

    [Parameter]
    public bool IsAdd { get; set; }

    public MForm? Form { get; set; }

    private AppChannelOptions _model = new();
    private bool _passwordShow;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _model = ExtensionPropertyHelper.ConvertToType<AppChannelOptions>(Value);
    }

    protected override void OnParametersSet()
    {
        if (_model.Provider == default && Value.HasProperty(nameof(AppChannelOptions.Provider)))
        {
            var provider = Value.GetProperty<string>(nameof(AppChannelOptions.Provider));

            _model.Provider = (AppChannelProviders)Enum.Parse(typeof(AppChannelProviders), provider);
        }
    }

    public async Task HandleChangeAsync()
    {
        Value = ExtensionPropertyHelper.ConvertToExtraProperty(_model);
        await ValueChanged.InvokeAsync(Value);
    }
}