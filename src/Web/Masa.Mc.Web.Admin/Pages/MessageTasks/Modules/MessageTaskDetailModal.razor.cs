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
    private GetMessageTaskHistoryInput _queryParam = new();
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));
    private List<MessageTaskHistoryDto> _historys = new();

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var _prefix = "DisplayName.MessageTaskHistory";
        _headers = new()
        {
            new() { Text = T($"{_prefix}{nameof(MessageTaskHistoryDto.Status)}"), Value = nameof(MessageTaskHistoryDto.Status), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskHistoryDto.SendTime)}"), Value = nameof(MessageTaskHistoryDto.SendTime), Sortable = false },
        };
    }

    public async Task OpenModalAsync(MessageTaskDto model)
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
        _info = await MessageTaskService.GetAsync(_entityId) ?? new();
        _historyInfo = _info.Historys[0];
        LoadData();
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

    private void Refresh()
    {
        _queryParam.Page = 1;
        LoadData();
    }

    private void HandleDatePickersAsync()
    {
        _datePickersShow = false;
        if (_dates.Count > 0) _queryParam.StartTime = _dates[0].ToDateTime(new TimeOnly(0, 0, 0));
        if (_dates.Count > 1) _queryParam.EndTime = _dates[1].ToDateTime(new TimeOnly(23, 59, 59));
        LoadData();
    }

    private void HandleDatePickersCancel()
    {
        _datePickersShow = false;
        _queryParam.StartTime = null;
        _queryParam.EndTime = null;
        _dates = new();
        LoadData();
    }

    private void LoadData()
    {
        var query = _info.Historys.AsQueryable();
        if (_queryParam.Status.HasValue) query = query.Where(x => x.Status == _queryParam.Status);
        if (_queryParam.StartTime.HasValue) query = query.Where(x => x.SendTime >= _queryParam.StartTime);
        if (_queryParam.EndTime.HasValue) query = query.Where(x => x.SendTime <= _queryParam.EndTime);
        _historys = query.OrderByDescending(x => x.CreationTime).Skip((_queryParam.Page - 1) * _queryParam.PageSize).Take(_queryParam.PageSize <= 0 ? int.MaxValue : _queryParam.PageSize).ToList();
        StateHasChanged();
    }

    private void HandleItemSelect(MessageTaskHistoryDto item)
    {
        _historyInfo = item;
    }

    private void HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        LoadData();
    }

    private async Task WithdrawnHistoryAsync()
    {
        var input = new WithdrawnMessageTaskHistoryInput
        {
            MessageTaskId = _entityId,
            HistoryId = _historyInfo.Id
        };
        await MessageTaskService.WithdrawnHistoryAsync(input);
        await SuccessMessageAsync(T("MessageTaskHistoryWithdrawnMessage"));
        await GetFormDataAsync();
    }

    private async Task HandleIsEnabledChanged(bool IsEnabled)
    {
        if (IsEnabled)
        {
            await MessageTaskService.EnabledAsync(new EnabledMessageTaskInput { MessageTaskId = _entityId });
            _info.IsEnabled = true;
        }
        else
        {
            await MessageTaskService.DisableAsync(new DisableMessageTaskInput { MessageTaskId = _entityId });
            _info.IsEnabled = false;
        }
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
}
