﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates;

public partial class AppTemplateManagement : AdminCompontentBase
{
    private AppTemplateUpsertModal _upsertModal = default!;
    private GetMessageTemplateInputDto _queryParam = new() { ChannelType = ChannelTypes.App };
    private PaginatedListDto<MessageTemplateDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private DateTimeOffset? _endTime;
    private DateTimeOffset? _startTime;

    private ChannelService ChannelService => McCaller.ChannelService;

    private MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override string? PageName { get; set; } = "MessageTemplateBlock";

    protected async override Task OnInitializedAsync()
    {
        _channelItems = await ChannelService.GetListByTypeAsync(ChannelTypes.App);
    }
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public List<DataTableHeader<MessageTemplateDto>> GetHeaders()
    {
        var prefix = "DisplayName.MessageTemplate";

        return new()
        {
            new() { Text = T($"{prefix}{nameof(MessageTemplateDto.Code)}"), Value = nameof(MessageTemplateDto.Code), Sortable = false, Width = "8rem" },
            new() { Text = T($"{prefix}{nameof(MessageTemplateDto.Title)}"), Value = nameof(MessageTemplateDto.Title), Sortable = false, Width = "13.125rem" },
            new() { Text = T($"{prefix}ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false, Width = "6.5625rem" },
            new() { Text = T("Modifier"), Value = nameof(MessageTemplateDto.ModifierName), Sortable = false, Width = "6.5625rem" },
            new() { Text = T("ModificationTime"), Value = nameof(MessageTemplateDto.ModificationTime), Sortable = true, Width = "13.125rem" },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Width = 105, Align = DataTableHeaderAlign.Center },
        };
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
}
