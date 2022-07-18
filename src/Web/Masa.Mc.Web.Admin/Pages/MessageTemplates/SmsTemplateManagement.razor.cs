﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates;

public partial class SmsTemplateManagement : AdminCompontentBase
{
    public List<DataTableHeader<MessageTemplateDto>> Headers { get; set; } = new();

    private SmsTemplateEditModal _editModal;
    private SmsTemplateCreateModal _createModal;
    private GetMessageTemplateInputDto _queryParam = new() { ChannelType = ChannelTypes.Sms };
    private PaginatedListDto<MessageTemplateDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = false;
    private bool isAnimate;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override async Task OnInitializedAsync()
    {
        var _prefix = "DisplayName.MessageTemplate";
        Headers = new()
        {
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.TemplateId)}"), Value = nameof(MessageTemplateDto.TemplateId), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.DisplayName)}"), Value = nameof(MessageTemplateDto.DisplayName), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.TemplateType)}"), Value = nameof(MessageTemplateDto.TemplateType), Sortable = false },
            new() { Text = T($"{_prefix}ChannelDisplayName"), Value = "ChannelDisplayName", Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.AuditStatus)}"), Value = nameof(MessageTemplateDto.AuditStatus), Sortable = false },
            new() { Text = T("Modifier"), Value = nameof(MessageTemplateDto.ModifierName), Sortable = false },
            new() { Text = T($"{_prefix}{nameof(MessageTemplateDto.ModificationTime)}"), Value = nameof(MessageTemplateDto.ModificationTime), Sortable = true },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };
        _channelItems = await ChannelService.GetListByTypeAsync(ChannelTypes.Sms);
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
        _queryParam = new() { ChannelType = ChannelTypes.Sms };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
        isAnimate = true;
    }
}
