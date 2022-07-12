// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageRecords.Modules;

public partial class MessageRecordDetailModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MessageRecordDto _messageRecord = new();
    private MessageTaskDto _messageTask = new();
    private MessageTaskHistoryDto _messageTaskHistory = new();
    private Guid _entityId;
    private bool _visible;

    MessageRecordService MessageRecordService => McCaller.MessageRecordService;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    MessageTaskHistoryService MessageTaskHistoryService => McCaller.MessageTaskHistoryService;

    public async Task OpenModalAsync(MessageRecordDto model)
    {
        _entityId = model.Id;
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        _messageRecord = await MessageRecordService.GetAsync(_entityId) ?? new();
        _messageTask = await MessageTaskService.GetAsync(_messageRecord.MessageTaskId) ?? new();
        _messageTaskHistory = await MessageTaskHistoryService.GetAsync(_messageRecord.MessageTaskHistoryId) ?? new();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private void ResetForm()
    {
        _messageRecord = new();
        _messageTask = new();
        _messageTaskHistory = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task HandleRetry()
    {
        LoadingText = T("SendingMessage");
        Loading = true;
        var inputDto = new RetryMessageRecordInputDto { MessageRecordId = _entityId };
        await MessageRecordService.RetryAsync(inputDto);
        Loading = false;
        await SuccessMessageAsync(T("MessageRecordRetryMessage"));
        await GetFormDataAsync();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
}
