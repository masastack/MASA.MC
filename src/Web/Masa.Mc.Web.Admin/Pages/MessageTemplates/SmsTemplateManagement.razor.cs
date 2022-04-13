namespace Masa.Mc.Web.Admin.Pages.MessageTemplates;

public partial class SmsTemplateManagement : AdminCompontentBase
{
    [Inject]
    public MessageTemplateCaller MessageTemplateCaller { get; set; } = default!;

    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    public List<DataTableHeader<MessageTemplateDto>> Headers { get; set; } = new();

    private SmsTemplateEditModal _editModal;
    private SmsTemplateCreateModal _createModal;
    private GetMessageTemplateInput _queryParam = new() { ChannelType = ChannelType.Sms };
    private PaginatedListDto<MessageTemplateDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private List<MessageTemplateStatus> _statusItems = Enum.GetValues(typeof(MessageTemplateStatus))
        .Cast<MessageTemplateStatus>().ToList();
    private List<MessageTemplateAuditStatus> _auditStatusItems = Enum.GetValues(typeof(MessageTemplateAuditStatus))
        .Cast<MessageTemplateAuditStatus>().ToList();
    private bool advanced = true;
    private bool _datePickersShow;
    private List<DateOnly> _dates = new List<DateOnly>{};
    private string DateRangeText => string.Join(" ~ ", _dates.Select(date => date.ToString("yyyy-MM-dd")));

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName:MessageTemplate";
        Headers = new()
        {
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.TemplateId)}"), Value = nameof(MessageTemplateDto.TemplateId), Sortable = false },
            new() { Text = T("DisplayName:ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.DisplayName)}"), Value = nameof(MessageTemplateDto.DisplayName), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.ModificationTime)}"), Value = nameof(MessageTemplateDto.ModificationTime), Sortable = true },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.AuditStatus)}"), Value = nameof(MessageTemplateDto.AuditStatus), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.Status)}"), Value = nameof(MessageTemplateDto.Status), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };
        _channelItems = await ChannelCaller.GetListByTypeAsync(ChannelType.Sms);
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
        _entities = (await MessageTemplateCaller.GetListAsync(_queryParam));
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

    private async Task HandlePaginationChange(int page)
    {
        _queryParam.Page = page;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new() { ChannelType = ChannelType.Sms };
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
