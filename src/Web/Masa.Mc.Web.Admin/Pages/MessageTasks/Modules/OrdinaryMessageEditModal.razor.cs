﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MForm _form;
    private MessageTaskUpsertDto _model = new() { ReceiverType = ReceiverTypes.Assign, EntityType = MessageEntityTypes.Ordinary };
    private Guid _entityId;
    private bool _visible;
    private List<ChannelDto> _channelItems = new();
    private ChannelTypes _channelType;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    ChannelService ChannelService => McCaller.ChannelService;
    MessageInfoService MessageInfoService => McCaller.MessageInfoService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public async Task OpenModalAsync(MessageTaskDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<MessageTaskUpsertDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await MessageTaskService.GetAsync(_entityId);
        if (dto != null)
        {
             _channelType = dto.Channel.Type;
            _model = dto.Adapt<MessageTaskUpsertDto>();
            _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
            var messageInfo = await MessageInfoService.GetAsync(_model.EntityId);
            if (messageInfo != null) _model.MessageInfo = messageInfo.Adapt<MessageInfoUpsertDto>();
        }
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync(bool isDraft)
    {
        try
        {
            _model.IsDraft = isDraft;
            if (!await _form.ValidateAsync())
            {
                return;
            }
            Loading = true;
            await MessageTaskService.UpdateAsync(_entityId, _model);
            Loading = false;
            await SuccessMessageAsync(T("MessageTaskCreateMessage"));
            _visible = false;
            ResetForm();
            if (OnOk.HasDelegate)
            {
                await OnOk.InvokeAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task HandleDelAsync()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), DeleteAsync);
    }

    private async Task DeleteAsync()
    {
        Loading = true;
        await MessageTaskService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskDeleteMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new() { ReceiverType = ReceiverTypes.Assign, EntityType = MessageEntityTypes.Ordinary };
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task HandleChannelTypeChangeAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(_channelType);
        if (_channelType != ChannelTypes.WebsiteMessage)
        {
            _model.ReceiverType = ReceiverTypes.Assign;
        }
    }
}