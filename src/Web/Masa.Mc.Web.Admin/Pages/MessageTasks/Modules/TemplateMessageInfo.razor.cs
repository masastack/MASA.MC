// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageInfo : AdminCompontentBase
{
    [Parameter]
    public Guid MessageTemplateId { get; set; }

    [Parameter]
    public MessageTaskDto MessageTask { get; set; } = new();

    public MessageTemplateDto MessageTemplate { get; set; } = new();

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override async Task OnParametersSetAsync()
    {
        MessageTemplate = await MessageTemplateService.GetAsync(MessageTemplateId) ?? new();
    }
}
