// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageRecords;

public partial class MessageRecordManagement : AdminCompontentBase
{
    public List<DataTableHeader<MessageRecordDto>> Headers { get; set; } = new();

    private MessageRecordDetailModal _detailModal = default!;
    private GetMessageRecordInputDto _queryParam = new() { TimeType = MessageRecordTimeTypes.SendTime };
    private PaginatedListDto<MessageRecordDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;
    private List<KeyValuePair<string, bool>> _successItems { get; set; } = new();
    private bool isAnimate;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageRecordService MessageRecordService => McCaller.MessageRecordService;

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName.MessageRecord";
        Headers = new()
        {
            new() { Text = T("DisplayName.MessageTaskReceiver"), Value = "Receiver", Sortable = false},
            new() { Text = T(nameof(MessageTaskReceiverDto.Email)), Value = nameof(MessageTaskReceiverDto.Email), Sortable = false},
            new() { Text = T("DisplayName.Channel"), Value = "ChannelDisplayName", Sortable = false},
            new() { Text = T($"{_prefix}{nameof(MessageRecordDto.DisplayName)}"), Value = nameof(MessageRecordDto.DisplayName), Sortable = false},
            new() { Text = T($"{_prefix}{nameof(MessageRecordDto.ExpectSendTime)}"), Value = nameof(MessageRecordDto.ExpectSendTime), Sortable = false},
            new() { Text = T($"{_prefix}{nameof(MessageRecordDto.SendTime)}"), Value = nameof(MessageRecordDto.SendTime), Sortable = false},
            new() { Text = T($"{_prefix}{nameof(MessageRecordDto.Success)}"), Value = nameof(MessageRecordDto.Success), Sortable = false},
            new() { Text = T($"{_prefix}{nameof(MessageRecordDto.FailureReason)}"), Value = nameof(MessageRecordDto.FailureReason), Sortable = false},
            new() { Text = T("Action"), Value = "Action", Sortable = false},
        };
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInputDto(99))).Result;
        _successItems = new()
        {
            new(T("Success"), true),
            new(T("Failure"), false)
        };
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

    private async Task HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new() { TimeType = MessageRecordTimeTypes.SendTime };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
        isAnimate = true;
    }
}
