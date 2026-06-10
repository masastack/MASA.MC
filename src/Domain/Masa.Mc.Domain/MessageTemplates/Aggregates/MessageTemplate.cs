// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTemplates.Aggregates;

public class MessageTemplate : FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }
    public string DisplayName { get; protected set; } = string.Empty;
    public string Code { get; protected set; } = string.Empty;
    public MessageContent MessageContent { get; protected set; } = default!;
    public string Example { get; protected set; } = string.Empty;
    public string TemplateId { get; protected set; } = string.Empty;
    public string Sign { get; protected set; } = string.Empty;
    public MessageTemplateStatuses Status { get; protected set; }
    public MessageTemplateAuditStatuses AuditStatus { get; protected set; }
    public DateTimeOffset? AuditTime { get; protected set; }
    public DateTimeOffset? InvalidTime { get; protected set; }
    public string AuditReason { get; protected set; } = string.Empty;
    public int TemplateType { get; protected set; }
    public long PerDayLimit { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public ICollection<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();
    public MessageTemplateUnsubscribeConfig UnsubscribeConfig { get; protected set; } = MessageTemplateUnsubscribeConfig.Disabled();
    public ExtraPropertyDictionary Options { get; set; } = new();

    private MessageTemplate() { }

    public MessageTemplate(
        Guid channelId,
        string displayName,
        string code,
        MessageContent messageContent,
        string example,
        string templateId,
        string sign,
        int templateType,
        long perDayLimit,
        MessageTemplateUnsubscribeConfig? unsubscribeConfig = null,
        MessageTemplateStatuses status = MessageTemplateStatuses.Normal,
        MessageTemplateAuditStatuses auditStatus = MessageTemplateAuditStatuses.WaitAudit,
        string auditReason = "",
        bool isStatic = false)
    {
        ChannelId = channelId;
        DisplayName = displayName;
        Code = code;
        Example = example;
        TemplateId = templateId;
        Sign = sign;
        TemplateType = templateType;
        PerDayLimit = perDayLimit;
        Status = status;
        IsStatic = isStatic;
        MessageContent = messageContent;
        SetAuditStatus(auditStatus, auditReason);
        ApplyUnsubscribeConfig(unsubscribeConfig ?? MessageTemplateUnsubscribeConfig.Disabled());

        Items = new List<MessageTemplateItem>();
    }

    public virtual void AddOrUpdateItem(string code, string mappingCode, string displayText, string description)
    {
        var existingItem = Items.SingleOrDefault(item => item.Code == code);

        if (existingItem == null)
        {
            Items.Add(new MessageTemplateItem(Id, code, mappingCode, displayText, description));
        }
        else
        {
            existingItem.SetContent(mappingCode, displayText, description);
        }
    }

    public virtual void SetAuditStatus(MessageTemplateAuditStatuses auditStatus, string auditReason = "")
    {
        AuditStatus = auditStatus;
        if (auditStatus != MessageTemplateAuditStatuses.WaitAudit)
        {
            AuditTime = DateTimeOffset.UtcNow;
            AuditReason = auditReason;
        }
    }

    public virtual void SetInvalid()
    {
        InvalidTime = DateTimeOffset.UtcNow;
        Status = MessageTemplateStatuses.Invalid;
    }

    public virtual void ConfigureUnsubscribe(MessageTemplateUnsubscribeConfig config)
    {
        ApplyUnsubscribeConfig(config);
    }

    private void ApplyUnsubscribeConfig(MessageTemplateUnsubscribeConfig config)
    {
        Check.NotNull(config, nameof(config));
        if (TemplateType == (int)SmsTemplateTypes.VerificationCode && config.Enabled)
        {
            throw new UserFriendlyException(errorCode: MessageTemplateExceptionCodes.VERIFICATION_CODE_TEMPLATE_CANNOT_ENABLE_UNSUBSCRIBE);
        }

        UnsubscribeConfig = config;
    }

    public void Remove()
    {
        AddDomainEvent(new RemoveTemplateMessageTasksDomainEvent(Id));
    }

    public string AppendUnsubscribeSuffix(string content)
    {
        var normalizedContent = content ?? string.Empty;
        var suffix = UnsubscribeConfig.BuildSuffix();
        if (string.IsNullOrEmpty(suffix))
        {
            return normalizedContent;
        }

        if (string.IsNullOrWhiteSpace(normalizedContent))
        {
            return suffix;
        }

        return $"{normalizedContent}{Environment.NewLine}{suffix}";
    }

    public bool IsWebsiteMessage
    {
        get
        {
            return Options.GetProperty<bool>(BusinessConsts.IS_WEBSITE_MESSAGE);
        }
    }
}
