// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Messages;

public partial class MessageRight
{
    [Parameter]
    public Guid? ChannelId { get; set; }

    [Parameter]
    public EventCallback<Guid> OnClick { get; set; }

    private GetWebsiteMessageInputDto _queryParam = new();
    private PaginatedListDto<WebsiteMessageDto> _entities = new();
    private ChannelDto _channel;

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;
    ChannelService ChannelService => McCaller.ChannelService;

    protected override async void OnParametersSet()
    {
        _queryParam.ChannelId = ChannelId;
        _channel = ChannelId.HasValue ? await ChannelService.GetAsync(ChannelId.Value) : null;
        await RefreshAsync();
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await WebsiteMessageService.GetListAsync(_queryParam));
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleOnClick(Guid id)
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(id);
        }
    }

    private async Task RefreshAsync()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task SearchKeyDown(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == "Enter")
        {
            await RefreshAsync();
        }
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new();
        await LoadData();
    }
    private async Task HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        await LoadData();
    }

    private async Task HandleShowAll()
    {
        _queryParam.IsRead = null;
        await RefreshAsync();
    }

    private async Task HandleShowUnread()
    {
        _queryParam.IsRead = false;
        await RefreshAsync();
    }

    private async Task HandleMarkAllRead()
    {
        var dto = _queryParam.Adapt<SetAllReadWebsiteMessageInputDto>();
        await WebsiteMessageService.SetAllReadAsync(dto);
        await SuccessMessageAsync(T("OperationSuccessfulMessage"));
        await LoadData();
    }
}
