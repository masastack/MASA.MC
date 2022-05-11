// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.MessageTemplates;

public partial class MessageTemplateSelect : AdminCompontentBase
{
    [Parameter]
    public Guid? Value { get; set; }

    [Parameter]
    public EventCallback<Guid?> ValueChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [Parameter]
    public EventCallback<MessageTemplateDto> OnSelectedItemUpdate { get; set; }

    private bool _isLoading;

    protected List<MessageTemplateDto> Items = new();

    private GetMessageTemplateInputDto _queryParam = new();

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    public async Task UpdateSearchInputAsync(string val)
    {
        if (Items.Count > 0) return;
        if (_isLoading) return;
        _isLoading = true;
        _queryParam.Filter = val;
        Items = (await MessageTemplateService.GetListAsync(_queryParam)).Result;
        _isLoading = false;
    }
}

