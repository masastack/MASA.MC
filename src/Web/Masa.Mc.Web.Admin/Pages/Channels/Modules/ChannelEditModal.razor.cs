// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Mapster;

namespace Masa.Mc.Web.Admin.Pages.Channels.Modules;

public partial class ChannelEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private ChannelCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;
    private List<ChannelType> channelTypeItems = Enum.GetValues(typeof(ChannelType))
        .Cast<ChannelType>().ToList();
    private ChannelExtraProperties _channelExtraPropertiesRef = default!;

    ChannelService ChannelService => McCaller.ChannelService;

    public async Task OpenModalAsync(ChannelDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<ChannelCreateUpdateDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await ChannelService.GetAsync(_entityId);
        _model = dto.Adapt<ChannelCreateUpdateDto>();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk()
    {
        await _channelExtraPropertiesRef.UpdateExtraPropertiesAsync();
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await ChannelService.UpdateAsync(_entityId, _model);
        Loading = false;
        _visible = false;
        ResetForm();
        await SuccessMessageAsync(T("ChannelEditMessage"));
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDel()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), DeleteAsync);
    }
    private async Task DeleteAsync()
    {
        Loading = true;
        await ChannelService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("ChannelDeleteMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
    private void ResetForm()
    {
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}