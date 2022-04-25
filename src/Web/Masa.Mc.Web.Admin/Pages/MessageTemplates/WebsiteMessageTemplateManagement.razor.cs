namespace Masa.Mc.Web.Admin.Pages.MessageTemplates;

public partial class WebsiteMessageTemplateManagement : AdminCompontentBase
{
    public List<DataTableHeader<MessageTemplateDto>> Headers { get; set; } = new();

    private WebsiteMessageTemplateEditModal _editModal;
    private WebsiteMessageTemplateCreateModal _createModal;
    private GetMessageTemplateInput _queryParam = new() { ChannelType = ChannelType.WebsiteMessage };
    private PaginatedListDto<MessageTemplateDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly> { };
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName.MessageTemplate";
        Headers = new()
        {
            new() { Text = T("DisplayName.ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.DisplayName)}"), Value = nameof(MessageTemplateDto.DisplayName), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.ModificationTime)}"), Value = nameof(MessageTemplateDto.ModificationTime), Sortable = true },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };
        _channelItems = await ChannelService.GetListByTypeAsync(ChannelType.WebsiteMessage);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await MessageTemplateService.GetListAsync(_queryParam));
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
        _queryParam = new() { ChannelType = ChannelType.WebsiteMessage };
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
}
