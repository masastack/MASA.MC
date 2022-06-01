﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateUpsertDto
{
    public MessageTemplateUpsertDto()
    {
        this.Items = new List<MessageTemplateItemDto>();
    }
    public Guid ChannelId { get; set; }
    public ChannelTypes ChannelType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Markdown { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public MessageTemplateStatuses Status { get; set; } = MessageTemplateStatuses.Normal;
    public MessageTemplateAuditStatuses AuditStatus { get; set; } = MessageTemplateAuditStatuses.Adopt;
    public DateTimeOffset? AuditTime { get; set; }
    public DateTimeOffset? InvalidTime { get; set; }
    public string AuditReason { get; set; } = string.Empty;
    public int TemplateType { get; set; }
    public long PerDayLimit { get; set; }
    public bool IsStatic { get; set; }
    public List<MessageTemplateItemDto> Items { get; set; }
    public MessageInfoUpsertDto MessageInfo { get; set; }
}