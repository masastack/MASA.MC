// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates;

public partial class WeixinWorkTemplateManagement : AdminCompontentBase
{
    [Inject]
    public I18n I18n { get; set; }

    private WeixinWorkUpsertModal _upsertModal = default!;
    private GetMessageTemplateInputDto _queryParam = new() { ChannelType = ChannelTypes.WeixinWork };
    private PaginatedListDto<MessageTemplateDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private DateTimeOffset? _endTime;
    private DateTimeOffset? _startTime;
    private List<DataTableHeader<MessageTemplateDto>> _headers = new();

    private ChannelService ChannelService => McCaller.ChannelService;

    private MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override string? PageName { get; set; } = "MessageTemplateBlock";

    protected async override Task OnInitializedAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(ChannelTypes.WeixinWork);

        LoadHeaders();

        I18n.CultureChanged += Changed;
    }
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private Task DateRangChangedAsync((DateTimeOffset? startDate, DateTimeOffset? endDate) args)
    {
        (_startTime, _endTime) = args;
        _queryParam.StartTime = _startTime?.UtcDateTime;
        _queryParam.EndTime = _endTime?.UtcDateTime;
        return RefreshAsync();
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await MessageTemplateService.GetListAsync(_queryParam));
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
        _queryParam = new() { ChannelType = ChannelTypes.App };
        await LoadData();
    }

    private void LoadHeaders()
    {
        var prefix = "DisplayName.MessageTemplate";
        _headers = new()
        {
            new() { Text = T($"{prefix}{nameof(MessageTemplateDto.Code)}"), Value = nameof(MessageTemplateDto.Code), Sortable = false },
            new() { Text = T($"{prefix}{nameof(MessageTemplateDto.TemplateType)}"), Value = nameof(MessageTemplateDto.TemplateType)},
            new() { Text = T($"{prefix}{nameof(MessageTemplateDto.Title)}"), Value = nameof(MessageTemplateDto.Title), Sortable = false },
            new() { Text = T($"{prefix}ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T("Modifier"), Value = nameof(MessageTemplateDto.ModifierName), Sortable = false },
            new() { Text = T("ModificationTime"), Value = nameof(MessageTemplateDto.ModificationTime), Sortable = true },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Width = 105, Align = DataTableHeaderAlign.Center },
        };
    }

    void Changed(object? sender, EventArgs args)
    {
        LoadHeaders();
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        I18n.CultureChanged -= Changed;
        await base.DisposeAsyncCore();
    }
}
