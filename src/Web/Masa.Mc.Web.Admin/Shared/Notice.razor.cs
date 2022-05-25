// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Shared;

public partial class Notice : AdminCompontentBase
{
    private GetWebsiteMessageInputDto _queryParam = new(5);
    private PaginatedListDto<WebsiteMessageDto> _entities = new();
    private HubConnection hubConnection;

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri($"{McApiOptions.McServiceBaseAddress}/signalr-hubs/notifications"))
            .Build();

        hubConnection.On(SignalRMethodConsts.GET_NOTIFICATION, async () =>
        {
            await LoadData();
            await InvokeAsync(StateHasChanged);
        });

        hubConnection.On(SignalRMethodConsts.CHECK_NOTIFICATION, async () =>
        {
            await WebsiteMessageService.CheckAsync();
            await InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
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
        _entities = (await WebsiteMessageService.GetListAsync(_queryParam));
        Loading = false;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        hubConnection?.DisposeAsync();
        await Task.CompletedTask;
    }
}
