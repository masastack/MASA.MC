// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class OrdinaryMessageInfo : AdminCompontentBase
{
    [Parameter]
    public Guid MessageInfoId { get; set; }

    [Parameter]
    public MessageTaskDto MessageTask { get; set; } = new();

    public MessageInfoDto MessageInfo { get; set; } = new();

    MessageInfoService MessageInfoService => McCaller.MessageInfoService;

    protected override async Task OnParametersSetAsync()
    {
        MessageInfo = await MessageInfoService.GetAsync(MessageInfoId) ?? new();
    }
}
