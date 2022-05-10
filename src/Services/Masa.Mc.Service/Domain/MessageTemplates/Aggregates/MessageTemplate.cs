// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public Guid ChannelId { get; protected set; }
    public string DisplayName { get; protected set; } = string.Empty;
    public string Title { get; protected set; } = string.Empty;
    public string Content { get; protected set; } = string.Empty;
    public string Example { get; protected set; } = string.Empty;
    public string TemplateId { get; protected set; } = string.Empty;
    public bool IsJump { get; protected set; }
    public string JumpUrl { get; protected set; } = string.Empty;
    public string Sign { get; protected set; } = string.Empty;
    public MessageTemplateStatuses Status { get; protected set; }
    public MessageTemplateAuditStatuses AuditStatus { get; protected set; }
    public DateTime? AuditTime { get; protected set; }
    public DateTime? InvalidTime { get; protected set; }
    public string AuditReason { get; protected set; } = string.Empty;
    public int TemplateType { get; protected set; }
    public long PerDayLimit { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public ICollection<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();
    public bool IsDeleted { get; protected set; }

    public MessageTemplate(
        Guid channelId,
        string displayName,
        string title,
        string content,
        string example,
        string templateId,
        bool isJump,
        string jumpUrl,
        string sign,
        int templateType,
        long perDayLimit,
        MessageTemplateStatuses status = MessageTemplateStatuses.Normal,
        MessageTemplateAuditStatuses auditStatus = MessageTemplateAuditStatuses.WaitAudit,
        string auditReason = "",
        bool isStatic = false)
    {
        ChannelId = channelId;
        DisplayName = displayName;
        Example = example;
        TemplateId = templateId;
        Sign = sign;
        TemplateType = templateType;
        PerDayLimit = perDayLimit;
        Status = status;
        IsStatic = isStatic;

        SetContent(title, content);
        SetJump(isJump, jumpUrl);
        SetAuditStatus(auditStatus, auditReason);

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

    public virtual void SetContent(
        string title,
        string content)
    {
        Title = title;
        Content = content;
    }

    public virtual void SetAuditStatus(MessageTemplateAuditStatuses auditStatus, string auditReason = "")
    {
        AuditStatus = auditStatus;
        if (auditStatus != MessageTemplateAuditStatuses.WaitAudit)
        {
            AuditTime = DateTime.UtcNow;
            AuditReason = auditReason;
        }
    }

    public virtual void SetInvalid()
    {
        InvalidTime = DateTime.UtcNow;
        Status = MessageTemplateStatuses.Invalid;
    }

    public virtual void SetJump(bool isJump, string jumpUrl)
    {
        IsJump = isJump;
        JumpUrl = isJump ? jumpUrl : string.Empty;
    }
}
