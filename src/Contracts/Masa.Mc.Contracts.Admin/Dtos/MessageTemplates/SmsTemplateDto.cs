// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class SmsTemplateDto
{
    public Guid ChannelId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public SmsTemplateTypes TemplateType { get; set; }
    public MessageTemplateAuditStatues AuditStatus { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string AuditReason { get; set; } = string.Empty;
}
