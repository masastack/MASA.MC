// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTemplates.Aggregates;

public class MessageTemplateItem : Entity<Guid>
{
    public Guid MessageTemplateId { get; protected set; }

    public string Code { get; protected set; } = string.Empty;

    public string MappingCode { get; protected set; } = string.Empty;

    public string DisplayText { get; protected set; } = string.Empty;

    public string Description { get; protected set; } = string.Empty;

    protected MessageTemplateItem(Guid messageTemplateId, string code) : this(messageTemplateId, code, "", "", "")
    {

    }

    protected internal MessageTemplateItem(
        Guid messageTemplateId,
        string code,
        string mappingCode,
        string displayText,
        string description)
    {
        MessageTemplateId = messageTemplateId;
        Code = code;

        SetContent(mappingCode, displayText, description);
    }

    public void SetContent(string mappingCode, string displayText, string description)
    {
        MappingCode = mappingCode;
        DisplayText = displayText;
        Description = description;
    }
}
