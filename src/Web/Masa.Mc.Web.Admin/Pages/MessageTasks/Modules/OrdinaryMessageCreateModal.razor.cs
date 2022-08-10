// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskUpsertModel _model = new() { EntityType = MessageEntityTypes.Ordinary };
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private List<MessageTaskReceiverDto> _selectReceivers = new();
    private List<MessageTaskReceiverDto> _importReceivers = new();
    private bool _selectReceiverType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;

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

    private async Task HandleSaveAsync()
    {
        _model.IsDraft = true;
        await HandleOkAsync();
    }

    private async Task HandleSendAsync()
    {
        _model.IsDraft = false;
        await HandleOkAsync();
    }

    private async Task HandleOkAsync()
    {
        SetReceivers();
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        var inputDto = _model.Adapt<MessageTaskUpsertDto>();
        await MessageTaskService.CreateAsync(inputDto);
        Loading = false;
        if (_model.IsDraft)
        {
            await SuccessMessageAsync(T("MessageTaskCreateMessage"));
        }
        else
        {
            await SuccessMessageAsync(T("MessageTaskSendMessage"));
        }
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
        _selectReceiverType = false;
        await _form.ResetValidationAsync();
    }

    private async Task HandleVisibleChanged(bool val)
    {
        if (!val) await HandleCancel();
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = _model.ChannelType.HasValue ? await ChannelService.GetListByTypeAsync(_model.ChannelType.Value) : new();
        if (_model.ChannelType != ChannelTypes.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverTypes.Assign;
        }
        else
        {
            _model.ReceiverType = default;
        }
    }

    private void HandleReceiverType(ReceiverTypes receiverType)
    {
        _model.ReceiverType = receiverType;
        _selectReceiverType = true;
        if (receiverType == ReceiverTypes.Broadcast)
        {
            _model.Step = 3;
        }
    }

    private void HandleReceiverBack()
    {
        if (_model.ChannelType != ChannelTypes.WebsiteMessage || !_selectReceiverType)
        {
            _model.Step = 1;
        }

        if (_selectReceiverType)
        {
            _selectReceiverType = false;
        }
    }

    private void HandleSendingRuleBack()
    {
        _model.Step = 2;
        if (_model.ReceiverType == ReceiverTypes.Broadcast)
        {
            _selectReceiverType = false;
        }
    }

    private async Task HandleNextStep()
    {
        SetReceivers();
        _model.IsDraft = false;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        _model.Step++;
    }

    private void SetReceivers()
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
    }
}