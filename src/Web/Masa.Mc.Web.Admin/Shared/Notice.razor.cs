// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Shared;

public partial class Notice : AdminCompontentBase
{
    private GetWebsiteMessageInputDto _queryParam = new(5);
    private PaginatedListDto<WebsiteMessageDto> _entities = new();

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

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
}
