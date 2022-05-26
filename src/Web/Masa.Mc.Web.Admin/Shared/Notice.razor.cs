// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Web.Admin.Store;

namespace Masa.Mc.Web.Admin.Shared;

public partial class Notice : AdminCompontentBase
{
    [Inject]
    public NoticeState NoticeState { get; set; } = default!;

    private GetWebsiteMessageInputDto _queryParam = new(5);
    //private PaginatedListDto<WebsiteMessageDto> _entities = new();
    private HubConnection hubConnection;

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

    protected override async Task OnInitializedAsync()
    {
        NoticeState.OnNoticeChanged += LoadData;

        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri($"{McApiOptions.McServiceBaseAddress}/signalr-hubs/notifications"))
            .Build();
        hubConnection.On(SignalRMethodConsts.GET_NOTIFICATION, async () =>
        {
            await LoadData();
        });

        hubConnection.On(SignalRMethodConsts.CHECK_NOTIFICATION, async () =>
        {
            await WebsiteMessageService.CheckAsync();
        });

        await hubConnection.StartAsync();

        await WebsiteMessageService.CheckAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    async Task LoadData()
    {
        var dtos = await WebsiteMessageService.GetListAsync(_queryParam);
        NoticeState.SetNotices(dtos.Result);
        await InvokeAsync(StateHasChanged);
    }

    public override void Dispose()
    {
        NoticeState.OnNoticeChanged -= LoadData;
        hubConnection?.DisposeAsync();
    }
}
