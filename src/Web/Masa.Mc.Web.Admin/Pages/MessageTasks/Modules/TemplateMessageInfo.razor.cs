// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class TemplateMessageInfo : AdminCompontentBase
{
    [Parameter]
    public Guid MessageTemplateId { get; set; }

    [Parameter]
    public MessageTaskDto MessageTask { get; set; } = new();

    [Parameter]
    public ExtraPropertyDictionary Variables { get; set; } = new();

    public MessageTemplateDto MessageTemplate { get; set; } = new();

    MessageTemplateService MessageTemplateService => McCaller.MessageTemplateService;

    protected override async Task OnParametersSetAsync()
    {
        MessageTemplate = await MessageTemplateService.GetAsync(MessageTemplateId) ?? new();
        await RenderMessageContent();
    }

    public async Task RenderMessageContent()
    {
        var startstr = "{{";
        var endstr = "}}";
        if (MessageTask.Channel.Type == ChannelTypes.Sms)
        {
            startstr = "${";
            endstr = "}";
            MessageTemplate.Content = $"【{MessageTemplate.Sign}】{MessageTemplate.Content}";
        }
        MessageTemplate.Title = await RenderAsync(MessageTemplate.Title, Variables, startstr, endstr);
        MessageTemplate.Content = await RenderAsync(MessageTemplate.Content, Variables, startstr, endstr);
    }

    public Task<string> RenderAsync(string context, ExtraPropertyDictionary model, string startstr, string endstr)
    {
        foreach (var item in model)
        {
            context = context.Replace($"{startstr}{item.Key}{endstr}", item.Value.ToString());
        }
        return Task.FromResult(context);
    }
}
