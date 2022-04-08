namespace MASA.MC.Admin.Pages.Channels;

public partial class ChannelManagement : AdminCompontentBase
{
    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    private ChannelEditModal _editModal;
    private ChannelCreateModal _createModal;
    private GetChannelInput _queryParam = new(5);
    private PaginatedListDto<ChannelDto> _entities = new();

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
        _entities = (await ChannelCaller.GetListAsync(_queryParam));
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
        _queryParam.DisplayName = string.Empty;
        await LoadData();
    }
}
