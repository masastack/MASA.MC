// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public sealed class MessageRecordDetailContentResolver
{
    private readonly ITemplateRenderer _templateRenderer;

    public MessageRecordDetailContentResolver(ITemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer;
    }

    public MessageRecordContentResolution Resolve(
        MessageRecordQueryModel record,
        MessageInfoQueryModel? messageInfo = null,
        MessageTemplateQueryModel? template = null)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (record.MessageEntityType == MessageEntityTypes.Ordinary)
        {
            return messageInfo is null
                ? MessageRecordContentResolution.Unavailable()
                : new MessageRecordContentResolution(
                    CreateContent(messageInfo.Title, messageInfo.Content, messageInfo.Markdown, messageInfo.IsJump,
                        messageInfo.JumpUrl, messageInfo.ExtraProperties),
                    MessageRecordContentSources.MessageInfo,
                    true);
        }

        if (record.MessageEntityType != MessageEntityTypes.Template)
        {
            return MessageRecordContentResolution.Unavailable();
        }

        if (record.ContentSnapshot is not null)
        {
            var snapshot = record.ContentSnapshot;
            return new MessageRecordContentResolution(
                CreateContent(snapshot.Title, snapshot.Content, snapshot.Markdown, snapshot.IsJump,
                    snapshot.JumpUrl, snapshot.ExtraProperties),
                MessageRecordContentSources.RecordSnapshot,
                true);
        }

        if (template is null)
        {
            return MessageRecordContentResolution.Unavailable();
        }

        var isAliyunSms = record.Channel?.Type == ChannelTypes.Sms &&
                          record.Channel.Provider == (int)SmsProviders.Aliyun;
        var start = isAliyunSms ? "${" : "{{";
        var end = isAliyunSms ? "}" : "}}";
        var extraProperties = new ExtraPropertyDictionary(template.ExtraProperties);
        foreach (var item in template.Options)
        {
            extraProperties[item.Key] = item.Value;
        }

        var content = _templateRenderer.Render(template.Content, record.Variables ?? new(), start, end);
        if (isAliyunSms)
        {
            var sign = record.ExtraProperties.GetProperty<string>(nameof(MessageTemplate.Sign));
            if (!string.IsNullOrWhiteSpace(sign))
            {
                content = $"【{sign}】{content}";
            }
        }

        return new MessageRecordContentResolution(
            CreateContent(
                _templateRenderer.Render(template.Title, record.Variables ?? new(), start, end),
                content,
                template.Markdown,
                template.IsJump,
                _templateRenderer.Render(template.JumpUrl, record.Variables ?? new(), start, end),
                extraProperties),
            MessageRecordContentSources.CurrentTemplateFallback,
            false);
    }

    private static MessageRecordContentDto CreateContent(
        string title,
        string content,
        string markdown,
        bool isJump,
        string jumpUrl,
        ExtraPropertyDictionary extraProperties)
    {
        return new MessageRecordContentDto
        {
            Title = title,
            Content = content,
            Markdown = markdown,
            IsJump = isJump,
            JumpUrl = jumpUrl,
            ExtraProperties = new ExtraPropertyDictionary(extraProperties)
        };
    }
}

public sealed record MessageRecordContentResolution(
    MessageRecordContentDto? Content,
    MessageRecordContentSources Source,
    bool IsReliable)
{
    public static MessageRecordContentResolution Unavailable()
    {
        return new MessageRecordContentResolution(null, MessageRecordContentSources.Unavailable, false);
    }
}
