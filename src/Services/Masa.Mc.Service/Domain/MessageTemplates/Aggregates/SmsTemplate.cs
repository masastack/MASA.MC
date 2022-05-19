// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;

public class SmsTemplate : FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }
    public string TemplateCode { get; protected set; } = string.Empty;
    public string TemplateName { get; protected set; } = string.Empty;
    public SmsTemplateTypes TemplateType { get; protected set; }
    public MessageTemplateAuditStatuses AuditStatus { get; set; }
    public string TemplateContent { get; protected set; } = string.Empty;
    public string AuditReason { get; protected set; } = string.Empty;

    public SmsTemplate(Guid channelId, string templateCode, string templateName, SmsTemplateTypes templateType, MessageTemplateAuditStatuses auditStatus, string templateContent, string auditReason)
    {
        ChannelId = channelId;
        TemplateCode = templateCode ?? string.Empty;
        TemplateName = templateName ?? string.Empty;
        TemplateType = templateType;
        AuditStatus = auditStatus;
        TemplateContent = templateContent ?? string.Empty;
        AuditReason = auditReason ?? string.Empty;
    }

    public void Update(string templateName, SmsTemplateTypes templateType, MessageTemplateAuditStatuses auditStatus, string templateContent, string auditReason)
    {
        TemplateName = templateName ?? string.Empty;
        TemplateType = templateType;
        AuditStatus = auditStatus;
        TemplateContent = templateContent ?? string.Empty;
        AuditReason = auditReason ?? string.Empty;
    }
}
