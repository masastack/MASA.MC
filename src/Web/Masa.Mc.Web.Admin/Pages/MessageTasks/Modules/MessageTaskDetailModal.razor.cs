﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskDetailModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private List<DataTableHeader<MessageTaskHistoryDto>> _headers = new();

    private MessageTaskDto _info = new();
    private MessageTaskHistoryDto _historyInfo = new();
    private Guid _entityId;
    private bool _visible;
    private GetMessageTaskHistoryInputDto _queryParam = new();
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));
    private PaginatedListDto<MessageTaskHistoryDto> _historys = new();

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;
    MessageTaskHistoryService MessageTaskHistoryService => McCaller.MessageTaskHistoryService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var _prefix = "DisplayName.MessageTaskHistory";
        _headers = new()
        {
            new() { Text = T($"{_prefix}{nameof(MessageTaskHistoryDto.Status)}"), Value = nameof(MessageTaskHistoryDto.Status), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskHistoryDto.TaskHistoryNo)}"), Value = nameof(MessageTaskHistoryDto.TaskHistoryNo), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskHistoryDto.SendTime)}"), Value = nameof(MessageTaskHistoryDto.SendTime), Sortable = false },
        };
    }

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
        if (_historys.Result.Any()) _historyInfo = _historys.Result[0];
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

    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private async Task Refresh()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task LoadData()
    {
        StateHasChanged();
        Loading = true;
        _historys = (await MessageTaskHistoryService.GetListAsync(_queryParam));
        Loading = false;
        StateHasChanged();
    }

    private void HandleItemSelect(MessageTaskHistoryDto item)
    {
        _historyInfo = item;
    }

    private async Task HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        await LoadData();
    }

    private async Task WithdrawnHistoryAsync()
    {
        var inputDto = new WithdrawnMessageTaskHistoryInputDto
        {
            HistoryId = _historyInfo.Id
        };
        Loading = true;
        await MessageTaskHistoryService.WithdrawnAsync(inputDto);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskHistoryWithdrawnMessage"));
        await GetFormDataAsync();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
}
