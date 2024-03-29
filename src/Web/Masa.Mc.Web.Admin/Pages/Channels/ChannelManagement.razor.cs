﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Channels;

public partial class ChannelManagement : AdminCompontentBase
{
    private ChannelEditModal _editModal = default!;
    private ChannelCreateModal _createModal = default!;
    private GetChannelInputDto _queryParam = new(999);
    private PaginatedListDto<ChannelDto> _entities = new();

    ChannelService ChannelService => McCaller.ChannelService;

    protected override string? PageName { get; set; } = "ChannelBlock";

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
        _entities = (await ChannelService.GetListAsync(_queryParam));
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

    private async Task HandleClearAsync()
    {
        _queryParam=new(15);
        await LoadData();
    }
}
