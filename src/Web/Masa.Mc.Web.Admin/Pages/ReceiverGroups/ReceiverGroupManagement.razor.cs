﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups;

public partial class ReceiverGroupManagement : AdminCompontentBase
{
    public List<DataTableHeader<ReceiverGroupDto>> Headers { get; set; } = new();

    private ReceiverGroupEditModal _editModal;
    private ReceiverGroupCreateModal _createModal;
    private GetReceiverGroupInputDto _queryParam = new(10);
    private PaginatedListDto<ReceiverGroupDto> _entities = new();

    ReceiverGroupService ReceiverGroupService => McCaller.ReceiverGroupService;

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName.ReceiverGroup";
        Headers = new()
        {
            new() { Text = T($"{_prefix}{nameof(ReceiverGroupDto.DisplayName)}"), Value = nameof(ReceiverGroupDto.DisplayName), Sortable = false },
            new() { Text = T($"{nameof(ReceiverGroupDto.Modifier)}"), Value = nameof(ReceiverGroupDto.Modifier), Sortable = false },
            new() { Text = T($"{nameof(MessageRecordDto.ModificationTime)}"), Value = nameof(ReceiverGroupDto.ModificationTime), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
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
        _entities = (await ReceiverGroupService.GetListAsync(_queryParam));
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
        _queryParam=new();
        await LoadData();
    }
}
