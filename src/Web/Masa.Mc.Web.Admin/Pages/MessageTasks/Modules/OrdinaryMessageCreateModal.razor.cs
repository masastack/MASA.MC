﻿// Copyright (c) MASA Stack All rights reserved.
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
    private List<string> _tabs = new();
    private string _tab = "";
    private bool _selectReceiverType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;

    protected override async Task OnInitializedAsync()
    {
        _tabs = new List<string> { T("DisplayName.MessageInfoContent"), T("DisplayName.MessageTaskReceiver"), T("DisplayName.MessageTaskSendingRule") };
        _tab = _tabs[0];
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
        SetReceivers();
        _model.IsDraft = isDraft;
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
            _tab = _tabs[2];
        }
    }

    private void HandleReceiverBack()
    {
        if (_model.ChannelType != ChannelTypes.WebsiteMessage || !_selectReceiverType)
        {
            _tab = _tabs[0];
        }

        if (_selectReceiverType)
        {
            _selectReceiverType = false;
        }
    }

    private void HandleSendingRuleBack()
    {
        _tab = _tabs[1];
        if (_model.ReceiverType == ReceiverTypes.Broadcast)
        {
            _selectReceiverType = false;
        }
    }

    private async Task HandleNextStep(int currentStep)
    {
        _model.Step = currentStep;
        SetReceivers();
        _model.IsDraft = false;
        if (!await _form.ValidateAsync())
        {
            return;
        }
        _tab = _tabs[currentStep];
    }

    private void SetReceivers()
    {
        _model.Receivers = _model.SelectReceiverType == MessageTaskSelectReceiverTypes.ManualSelection ? _selectReceivers : _importReceivers;
    }
}