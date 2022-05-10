// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private ChannelUpsertDto _model = new();
    private bool _visible;
    private List<ChannelTypes> channelTypeItems = Enum.GetValues(typeof(ChannelTypes))
        .Cast<ChannelTypes>().ToList();
    private ChannelExtraProperties _channelExtraPropertiesRef = default!;

    int _step = 1;

    ChannelService ChannelService => McCaller.ChannelService;

    public async Task OpenModalAsync()
    {
        _model.Type = ChannelTypes.Sms;
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private void HandleSelectType(ChannelTypes Type)
    {
        _model.Type = Type;
        _step++;
    }

    private async Task HandleNextStepAsync()
    {
        if (_model.Type == default)
        {
            await WarningAsync(T("Description.Channel.Type.Required"));
            return;
        }
        _step++;
    }

    private async Task HandleOkAsync()
    {
        
        await _channelExtraPropertiesRef.UpdateExtraPropertiesAsync();
        if (!await _form.ValidateAsync())
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
}
