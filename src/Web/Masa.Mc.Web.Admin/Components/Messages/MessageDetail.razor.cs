// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Messages;

public partial class MessageDetail
{
    [Parameter]
    public Guid EntityId { get; set; }

    private WebsiteMessageDto _entity = new();

    WebsiteMessageService WebsiteMessageService => McCaller.WebsiteMessageService;

    protected override async void OnParametersSet()
    {
        _entity = await WebsiteMessageService.GetAsync(EntityId) ?? new();
        StateHasChanged();
    }
}
