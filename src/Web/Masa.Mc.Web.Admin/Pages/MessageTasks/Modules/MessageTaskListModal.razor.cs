namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageTaskListModal : AdminCompontentBase
{
    public List<DataTableHeader<MessageTaskDto>> Headers { get; set; } = new();

    private bool _visible;
    private SmsTemplateEditModal _editModal;
    private GetMessageTaskInput _queryParam = new();
    private PaginatedListDto<MessageTaskDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName.MessageTask";
        Headers = new()
        {
            new() { Text = "", Value = "Draft", Sortable = false, Width=50},
            new() { Text = T("DisplayName.MessageInfoTitle"), Value = "MessageInfoTitle", Sortable = false},
            new() { Text = T("DisplayName.ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskDto.EntityType)}"), Value = nameof(MessageTaskDto.EntityType), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTaskDto.SendTime)}"), Value = nameof(MessageTaskDto.SendTime), Sortable = false },
            new() { Text = T($"ModificationTime"), Value = nameof(MessageTaskDto.ModificationTime), Sortable = true },
            new() { Text = T($"IsEnabled"), Value = nameof(MessageTaskDto.IsEnabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInput(99))).Result;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task OpenModalAsync()
    {
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
        _queryParam = new();
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
    }

    private async Task HandleDatePickersAsync()
    {
        _datePickersShow = false;
        await LoadData();
    }

    private async Task HandleDatePickersCancel()
    {
        _datePickersShow = false;
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
}
