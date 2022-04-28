namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskListModal : AdminCompontentBase
{
    public List<DataTableHeader<MessageTaskDto>> Headers { get; set; } = new();

    private bool _visible;
    private TemplateMessageEditModal _templateEditModal = default!;
    private OrdinaryMessageEditModal _ordinaryEditModal = default!;
    private MessageTaskSendModal _sendModal = default!;
    private MessageTaskDetailModal _detailModal = default!;
    private SendTestMessageModal _sendTestModal = default!;
    private GetMessageTaskInput _queryParam = new() { TimeType = MessageTaskTimeType.ModificationTime };
    private PaginatedListDto<MessageTaskDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var _prefix = "DisplayName.MessageTask";
        Headers = new()
        {
            new() { Text = "", Value = "Draft", Sortable = false, Width = 50 },
            new() { Text = T($"{_prefix}{nameof(MessageTaskDto.DisplayName)}"), Value = nameof(MessageTaskDto.DisplayName), Sortable = false },
            new() { Text = T("DisplayName.ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskDto.EntityType)}"), Value = nameof(MessageTaskDto.EntityType), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskDto.SendTime)}"), Value = nameof(MessageTaskDto.SendTime), Sortable = false },
            new() { Text = T($"ModificationTime"), Value = nameof(MessageTaskDto.ModificationTime), Sortable = true },
            new() { Text = T($"IsEnabled"), Value = nameof(MessageTaskDto.IsEnabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = "left" },
        };
    }

    public async Task OpenModalAsync()
    {
        await LoadData();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await MessageTaskService.GetListAsync(_queryParam));
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInput(99))).Result;
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleOk()
    {
        await LoadData();
    }

    private async Task SearchKeyDown(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == "Enter")
        {
            await RefreshAsync();
        }
    }

    private async Task RefreshAsync()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new() { TimeType = MessageTaskTimeType.ModificationTime };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
    }

    private async Task HandleDatePickersAsync()
    {
        _datePickersShow = false;
        if (_dates.Count > 0) _queryParam.StartTime = _dates[0].ToDateTime(new TimeOnly(0, 0, 0));
        if (_dates.Count > 1) _queryParam.EndTime = _dates[1].ToDateTime(new TimeOnly(23, 59, 59));
        await LoadData();
    }

    private async Task HandleDatePickersCancel()
    {
        _datePickersShow = false;
        _queryParam.StartTime = null;
        _queryParam.EndTime = null;
        _dates = new();
        await LoadData();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }

    private void HandleCancel()
    {
        _visible = false;
    }

    private async Task HandleEditAsync(MessageTaskDto model)
    {
        if (model.EntityType == MessageEntityType.Ordinary)
        {
            await _ordinaryEditModal.OpenModalAsync(model);
        }
        if (model.EntityType == MessageEntityType.Template)
        {
            await _templateEditModal.OpenModalAsync(model);
        }
    }
}
