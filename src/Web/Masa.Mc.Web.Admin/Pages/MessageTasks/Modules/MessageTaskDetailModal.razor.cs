// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskDetailModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private MessageTaskDto _info = new();
    private MessageTaskHistoryDto _historyInfo = new();
    private Guid _entityId;
    private bool _visible;
    private GetMessageTaskHistoryInputDto _queryParam = new();

    private PaginatedListDto<MessageTaskHistoryDto> _histories = new();
    private DateOnly? _endTime;
    private DateOnly? _startTime;
    private List<MessageTaskReceiverDto> _receiverUsers = new();

    private MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    private MessageTaskHistoryService MessageTaskHistoryService => McCaller.MessageTaskHistoryService;

    protected override string? PageName { get; set; } = "MessageTaskBlock";


    public async Task OpenModalAsync(MessageTaskDto model)
    {
        _entityId = model.Id;
        _queryParam.MessageTaskId = _entityId;
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        _info = await MessageTaskService.GetAsync(_entityId) ?? new();
        await LoadData();
        if (_histories.Result.Any())
        {
            _historyInfo = _histories.Result[0];

            await GetReceiversAsync(_historyInfo.Id);
        }
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
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
        _info = new();
        _historyInfo = new();
        _queryParam = new() { Page = 1 };
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private Task OnDateChanged((DateOnly? startDate,DateOnly? endDate) args)
    {
        (_startTime, _endTime) = args;
        _queryParam.StartTime = _startTime?.ToDateTime(TimeOnly.MinValue);
        _queryParam.EndTime = _endTime?.ToDateTime(TimeOnly.MaxValue);
        return Refresh();
    }

    private async Task Refresh()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task LoadData()
    {
        Loading = true;
        _histories = (await MessageTaskHistoryService.GetListAsync(_queryParam));
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleItemSelect(MessageTaskHistoryDto item)
    {
        _historyInfo = item;
        await GetReceiversAsync(_historyInfo.Id);
    }

    private async Task HandlePageChanged(int page)
    {
        _queryParam.Page = page;
        await LoadData();
    }

    private async Task HandlePageSizeChanged(int pageSize)
    {
        _queryParam.PageSize = pageSize;
        await LoadData();
    }

    private async Task HandleWithdrawnAsync()
    {
        await ConfirmAsync(T("TaskWithdrawnConfirmationMessage", $"[{_info.DisplayName}]"), WithdrawnAsync);
    }

    private async Task WithdrawnAsync()
    {
        Loading = true;
        await MessageTaskService.WithdrawnAsync(_info.Id);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskWithdrawnMessage"));
        await GetFormDataAsync();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task GetReceiversAsync(Guid id)
    {
        _receiverUsers = await MessageTaskHistoryService.GetReceiversAsync(id);
    }
}
