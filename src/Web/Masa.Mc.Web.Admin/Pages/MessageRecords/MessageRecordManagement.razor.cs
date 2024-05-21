// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageRecords;

public partial class MessageRecordManagement : AdminCompontentBase
{
    private MessageRecordDetailModal _detailModal = default!;
    private GetMessageRecordInputDto _queryParam = new();
    private PaginatedListDto<MessageRecordDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private List<KeyValuePair<string, bool>> _successItems = new();
    private ChannelService ChannelService => McCaller.ChannelService;
    private MessageRecordService MessageRecordService => McCaller.MessageRecordService;
    private DateTimeOffset? _endTime;
    private DateTimeOffset? _startTime;

    protected override string? PageName { get; set; } = "MessageRecordBlock";

    protected async override Task OnInitializedAsync()
    {
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInputDto(99))).Result;
        _successItems = new()
        {
            new(T("Success"), true),
            new(T("Failure"), false)
        };
    }

    private void SetDefaultParam()
    {
        var defaultQueryParam = new GetMessageRecordInputDto()
        {
            TimeType = MessageRecordTimeTypes.ExpectSendTime,
            StartTime = DateTime.Now.Date,
            EndTime = DateTime.Now.Date.AddDays(1)
        };
        _queryParam = defaultQueryParam;
        _endTime = defaultQueryParam.EndTime;
        _startTime = defaultQueryParam.StartTime;
    }

    public List<DataTableHeader<MessageRecordDto>> GetHeaders() 
    {
        string prefix = "DisplayName.MessageRecord";

        return new()
        {
            new() { Text = T("DisplayName.MessageTaskReceiver"), Value = "Receiver", Sortable = false, Width = "8rem" },
            new() { Text = T(nameof(MessageTaskReceiverDto.Email)), Value = nameof(MessageTaskReceiverDto.Email), Sortable = false, Width = "8rem" },
            new() { Text = T("DisplayName.Channel"), Value = "ChannelDisplayName", Sortable = false, Width = "6rem" },
            new() { Text = T($"{prefix}{nameof(MessageRecordDto.DisplayName)}"), Value = nameof(MessageRecordDto.DisplayName), Sortable = false, Width = "8rem" },
            new() { Text = T($"{prefix}{nameof(MessageRecordDto.ExpectSendTime)}"), Value = nameof(MessageRecordDto.ExpectSendTime), Sortable = false, Width = "10rem" },
            new() { Text = T($"{prefix}{nameof(MessageRecordDto.SendTime)}"), Value = nameof(MessageRecordDto.SendTime), Sortable = false, Width = "8rem" },
            new() { Text = T($"{prefix}{nameof(MessageRecordDto.Success)}"), Value = nameof(MessageRecordDto.Success), Sortable = false, Width = "7.5rem" },
            new() { Text = T($"{prefix}{nameof(MessageRecordDto.FailureReason)}"), Value = nameof(MessageRecordDto.FailureReason), Sortable = false, Width = "6.5625rem" },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Width = 105, Align=DataTableHeaderAlign.Center },
        };
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            SetDefaultParam();

            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private Task DateRangChangedAsync((DateTimeOffset? startDate, DateTimeOffset? endDate) args)
    {
        (_startTime, _endTime) = args;
        _queryParam.StartTime = _startTime?.UtcDateTime;
        _queryParam.EndTime = _endTime?.UtcDateTime;
        return RefreshAsync();
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await MessageRecordService.GetListAsync(_queryParam));
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleOk()
    {
        await LoadData();
    }

    private async Task RefreshAsync()
    {
        _queryParam.Page = 1;
        await LoadData();
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

    private async Task HandleClearAsync()
    {
        SetDefaultParam();
        await LoadData();
    }
}
