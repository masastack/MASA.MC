﻿namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups;

public partial class ReceiverGroupManagement : AdminCompontentBase
{
    [Inject]
    public ReceiverGroupCaller ReceiverGroupCaller { get; set; } = default!;

    private ReceiverGroupEditModal _editModal;
    private ReceiverGroupCreateModal _createModal;
    private GetReceiverGroupInput _queryParam = new(11);
    private PaginatedListDto<ReceiverGroupDto> _entities = new();

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
        _entities = (await ReceiverGroupCaller.GetListAsync(_queryParam));
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

    private async Task HandlePaginationChange(int page)
    {
        _queryParam.Page = page;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam=new();
        await LoadData();
    }
}
