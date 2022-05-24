// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
    private GetMessageTaskInputDto _queryParam = new() { TimeType = MessageTaskTimeTypes.ModificationTime };
    private PaginatedListDto<MessageTaskDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;

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
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInputDto(99))).Result;
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
        _queryParam = new() { TimeType = MessageTaskTimeTypes.ModificationTime };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
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
        if (model.EntityType == MessageEntityTypes.Ordinary)
        {
            await _ordinaryEditModal.OpenModalAsync(model);
        }
        if (model.EntityType == MessageEntityTypes.Template)
        {
            await _templateEditModal.OpenModalAsync(model);
        }
    }
}
