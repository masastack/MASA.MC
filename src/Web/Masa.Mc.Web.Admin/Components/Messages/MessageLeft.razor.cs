// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Messages;

public partial class MessageLeft : AdminCompontentBase
{
    [Parameter]
    public EventCallback<Guid?> OnClick { get; set; }

    private List<WebsiteMessageChannelListDto> _entities = new();

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadData()
    {
        Loading = true;
        _entities = (await WebsiteMessageService.GetChannelListAsync());
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleOnClick(Guid? channelId)
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(channelId);
        }
    }
}
