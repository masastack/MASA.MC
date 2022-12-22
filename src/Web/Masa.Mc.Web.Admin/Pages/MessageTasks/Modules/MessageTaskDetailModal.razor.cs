// Copyright (c) MASA Stack All rights reserved.
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

    private async Task HandleCancel()
    {
        _visible = false;
        ResetForm();
        await JsInvokeAsync("util.scrollTop", null, $"{Id}_dialog-box");
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
    }

    private async Task HandleVisibleChanged(bool val)
    {
        if (!val) await HandleCancel();
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
}
