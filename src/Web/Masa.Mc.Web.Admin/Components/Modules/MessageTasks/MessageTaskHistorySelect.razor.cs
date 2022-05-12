// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.MessageTasks;

public partial class MessageTaskHistorySelect : AdminCompontentBase
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
    public EventCallback<MessageTaskHistoryDto> OnSelectedItemUpdate { get; set; }

    private bool _isLoading;

    protected List<MessageTaskHistoryDto> Items = new();

    private GetMessageTaskHistoryInputDto _queryParam = new();

    MessageTaskHistoryService MessageTaskHistoryService => McCaller.MessageTaskHistoryService;

    public async Task UpdateSearchInputAsync(string val)
    {
        if (Items.Count > 0) return;
        if (_isLoading) return;
        _isLoading = true;
        _queryParam.Filter = val;
        Items = (await MessageTaskHistoryService.GetListAsync(_queryParam)).Result;
        _isLoading = false;
    }
}