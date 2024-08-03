// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm? _form;
    private ChannelUpsertDto _model = new();
    private bool _visible;
    private List<string> _colors = new List<string> { "purple", "green", "yellow", "red", "blue" };
    private ChannelExtraProperties _channelExtraPropertiesRef = default!;
    
    int _step = 1;

    Dictionary<string, object> _svgAttributes = new Dictionary<string, object> { ["viewBox"] = "0 0 1024 1024" };

    ChannelService ChannelService => McCaller.ChannelService;

    protected override string? PageName { get; set; } = "ChannelBlock";

    private int _nextStep
    {
        get
        {
            return _model.Type == ChannelTypes.App ? 2 : 3;
        }
    }

    private int _previousStep
    {
        get
        {
            if (_step == 3)
            {
                return _model.Type == ChannelTypes.App ? 2 : 1;
            }
            else
            {
                return 1;
            }
        }
    }

    public async Task OpenModalAsync()
    {
        _model.Color = _colors[0];
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });

        _form?.ResetValidation();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private void HandleSelectType(ChannelTypes Type)
    {
        _model.Type = Type;

        _step = _nextStep;
    }

    private async Task HandleOkAsync()
    {
        Check.NotNull(_form, "form not found");

        await _channelExtraPropertiesRef.UpdateExtraPropertiesAsync();
        if (!_form.Validate() || !_channelExtraPropertiesRef.Validate())
        {
            return;
        }
        Loading = true;
        await ChannelService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("ChannelCreateMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _step = 1;
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private void HandleAppSelectType(AppChannelProviders Type)
    {
        _model.ExtraProperties.SetProperty(nameof(AppChannelOptions.Provider), (int)Type);
        _step = 3;
    }
}
