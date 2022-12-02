// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

public class MessageData : ValueObject
{
    public MessageContent MessageContent { get; protected set; } = default!;

    public MessageEntityTypes MessageType { get; protected set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return MessageContent;
        yield return MessageType;
    }

    public MessageData(MessageContent messageContent, MessageEntityTypes messageEntityTypes)
    {
        MessageContent = messageContent;
        MessageType = messageEntityTypes;
    }

    public T GetDataValue<T>(string name)
    {
        return ExtraProperties.GetProperty<T>(name);
    }

    public void SetDataValue(string name, string value)
    {
        ExtraProperties.SetProperty(name, value);
    }

    public void RenderContent(ExtraPropertyDictionary variables, string startstr = "{{", string endstr = "}}")
    {
        MessageContent = new MessageContent(Render(MessageContent.Title, variables, startstr, endstr)
            , Render(MessageContent.Content, variables, startstr, endstr)
            , MessageContent.Markdown
            , MessageContent.IsJump
            , Render(MessageContent.JumpUrl, variables, startstr, endstr));
    }

    private string Render(string context, ExtraPropertyDictionary variables, string startstr, string endstr)
    {
        foreach (var item in variables)
        {
            context = context.Replace($"{startstr}{item.Key}{endstr}", item.Value?.ToString() ?? string.Empty);
        }
        return context;
    }
}
