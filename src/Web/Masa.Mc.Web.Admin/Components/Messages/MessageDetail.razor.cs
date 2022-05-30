// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Messages;

public partial class MessageDetail
{
    [Parameter]
    public Guid MessageId { get; set; }

    [Parameter]
    public EventCallback OnBack { get; set; }

    private WebsiteMessageDto _entity = new();

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

    protected override async void OnParametersSet()
    {
        await GetFormDataAsync(MessageId);
    }

    private async Task GetFormDataAsync(Guid id)
    {
        _entity = await WebsiteMessageService.GetAsync(id) ?? new();
        if (!_entity.IsRead)
        {
            await WebsiteMessageService.ReadAsync(new ReadWebsiteMessageInputDto { Id = id });
        }
        StateHasChanged();
    }

    private async Task HandleDelAsync()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), DeleteAsync);
    }

    private async Task DeleteAsync()
    {
        Loading = true;
        await WebsiteMessageService.DeleteAsync(MessageId);
        Loading = false;
        await SuccessMessageAsync(T("DeletedSuccessfullyMessage"));
        await HandleOnBack();
    }

    private async Task HandleOnBack()
    {
        if (OnBack.HasDelegate)
        {
            await OnBack.InvokeAsync();
        }
    }

    private async Task HandlePrevAndNext(Guid id)
    {
        if (id == default) return;
        await GetFormDataAsync(id);
    }
}
