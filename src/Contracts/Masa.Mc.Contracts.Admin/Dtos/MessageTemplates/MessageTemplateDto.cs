﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateDto : AuditEntityDto<Guid, Guid>
{
    public MessageTemplateDto()
    {
        this.Items = new List<MessageTemplateItemDto>();
    }
    public Guid ChannelId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public MessageTemplateStatues Status { get; set; }
    public MessageTemplateAuditStatues AuditStatus { get; set; }
    public DateTime? AuditTime { get; set; }
    public DateTime? InvalidTime { get; set; }
    public string AuditReason { get; set; } = string.Empty;
    public int TemplateType { get; set; }
    public long DayLimit { get; set; }
    public bool IsStatic { get; set; }
    public List<MessageTemplateItemDto> Items { get; set; }
    public ChannelDto Channel { get; set; }
}