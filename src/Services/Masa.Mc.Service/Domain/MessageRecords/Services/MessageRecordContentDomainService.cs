// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Services;

public class MessageRecordContentDomainService : DomainService
{
    private readonly ITemplateRenderer _templateRenderer;

    public MessageRecordContentDomainService(
        IDomainEventBus eventBus,
        ITemplateRenderer templateRenderer) : base(eventBus)
    {
        _templateRenderer = templateRenderer;
    }

    public MessageContent CreateForReceiver(MessageData messageData, ExtraPropertyDictionary variables)
    {
        ArgumentNullException.ThrowIfNull(messageData);

        return Create(messageData.RenderForReceiver(variables ?? new()));
    }

    public MessageContent Create(MessageData renderedData)
    {
        ArgumentNullException.ThrowIfNull(renderedData);

        var content = renderedData.MessageContent;
        var extraProperties = new ExtraPropertyDictionary(content.ExtraProperties);
        foreach (var item in renderedData.ExtraProperties)
        {
            extraProperties[item.Key] = item.Value;
        }

        return new MessageContent(
            content.Title,
            content.Content,
            content.Markdown,
            content.IsJump,
            content.JumpUrl,
            extraProperties).DeepCopy();
    }

    public MessageContent Create(MessageContent content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return content.DeepCopy();
    }

    public MessageContent CreateTemplate(MessageTemplate template, ExtraPropertyDictionary variables)
    {
        ArgumentNullException.ThrowIfNull(template);

        var source = template.MessageContent;
        var extraProperties = new ExtraPropertyDictionary(source.ExtraProperties);
        foreach (var item in template.Options)
        {
            extraProperties[item.Key] = item.Value;
        }

        return new MessageContent(
            _templateRenderer.Render(source.Title, variables ?? new()),
            _templateRenderer.Render(source.Content, variables ?? new()),
            source.Markdown,
            source.IsJump,
            _templateRenderer.Render(source.JumpUrl, variables ?? new()),
            extraProperties).DeepCopy();
    }

    public MessageContent CreateTemplateWithContent(MessageTemplate template, string content)
    {
        var snapshot = CreateTemplate(template, new ExtraPropertyDictionary());
        return new MessageContent(
            snapshot.Title,
            content ?? string.Empty,
            snapshot.Markdown,
            snapshot.IsJump,
            snapshot.JumpUrl,
            new ExtraPropertyDictionary(snapshot.ExtraProperties));
    }

    public MessageContent CreateSms(
        MessageTemplate template,
        ExtraPropertyDictionary variables,
        SmsProviders provider,
        string sign)
    {
        ArgumentNullException.ThrowIfNull(template);

        var convertedVariables = template.ConvertVariables(variables);
        var start = provider == SmsProviders.Aliyun ? "${" : "{{";
        var end = provider == SmsProviders.Aliyun ? "}" : "}}";
        var source = template.MessageContent;
        var title = _templateRenderer.Render(source.Title, convertedVariables, start, end);
        var content = _templateRenderer.Render(source.Content, convertedVariables, start, end);
        var jumpUrl = _templateRenderer.Render(source.JumpUrl, convertedVariables, start, end);

        if (provider == SmsProviders.Aliyun && !string.IsNullOrWhiteSpace(sign))
        {
            content = $"【{sign}】{content}";
        }

        content = template.AppendUnsubscribeSuffix(content);

        return new MessageContent(
            title,
            content,
            source.Markdown,
            source.IsJump,
            jumpUrl,
            source.ExtraProperties).DeepCopy();
    }
}
