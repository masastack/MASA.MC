// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskUpsertDto _model = new() { EntityType = MessageEntityTypes.Ordinary };
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelTypes _channelType;
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();
    StringNumber _tabIndex = 0;

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

    private async Task HandleCancel()
    {
        _visible = false;
        await ResetForm();
    }

    private async Task HandleOkAsync(bool isDraft)
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
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
        await ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task ResetForm()
    {
        _model = new() { EntityType = MessageEntityTypes.Ordinary };
        _selectReceivers = new();
        _importReceivers = new();
        await _form.ResetValidationAsync();
    }

    private async Task HandleVisibleChanged(bool val)
    {
        if (!val) await HandleCancel();
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
        if (_channelType != ChannelTypes.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverTypes.Assign;
        }
    }

    private void HandleReceiverType(ReceiverTypes receiverType)
    {
        _model.ReceiverType = receiverType;
        if (receiverType == ReceiverTypes.Broadcast)
        {
            _tabIndex = 1;
        }
    }
}