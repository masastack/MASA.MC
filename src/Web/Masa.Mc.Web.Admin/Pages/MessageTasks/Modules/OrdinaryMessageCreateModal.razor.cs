// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskCreateUpdateDto _model = new() { ReceiverType = ReceiverType.Assign, EntityType = MessageEntityType.Ordinary };
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelType _channelType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public async Task OpenModalAsync()
    {
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

    private async Task HandleOkAsync(bool isDraft)
    {
        _model.IsDraft = isDraft;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await MessageTaskService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskCreateMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new() { ReceiverType = ReceiverType.Assign, EntityType = MessageEntityType.Ordinary };
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
        if (_channelType != ChannelType.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverType.Assign;
        }
    }
}