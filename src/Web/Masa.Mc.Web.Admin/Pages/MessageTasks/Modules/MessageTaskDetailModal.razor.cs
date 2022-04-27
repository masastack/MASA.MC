namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskDetailModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private List<DataTableHeader<MessageTaskHistoryDto>> _headers = new();

    private MForm _form = default!;
    //private SendMessageTaskInput _model = new();
    private MessageTaskDto _info = new();
    private MessageTaskHistoryDto _historyInfo = new();
    private Guid _entityId;
    private bool _visible;
    private GetMessageTaskHistoryInput _queryParam = new();
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));

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
        //_model = _info.Adapt<SendMessageTaskInput>();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    //private async Task HandleOkAsync()
    //{
    //    if (!await _form.ValidateAsync())
    //    {
    //        return;
    //    }
    //    Loading = true;
    //    await MessageTaskService.SendAsync(_model);
    //    Loading = false;
    //    await SuccessMessageAsync(T("MessageTaskEditMessage"));
    //    _visible = false;
    //    ResetForm();
    //    if (OnOk.HasDelegate)
    //    {
    //        await OnOk.InvokeAsync();
    //    }
    //}

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
        //_model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private void Refresh()
    {
        _queryParam.Page = 1;
        //await LoadData();
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
        StateHasChanged();
    }

    private void HandleItemSelect(MessageTaskHistoryDto item)
    {
        _historyInfo = item;
    }
}
