// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class MessageTemplateQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid ChannelId { get; set; }

    public ChannelQueryModel Channel { get; set; } = default!;

    public string DisplayName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Markdown { get; set; } = string.Empty;

    public string Example { get; set; } = string.Empty;

    public string TemplateId { get; set; } = string.Empty;

    public bool IsJump { get; set; }

    public string JumpUrl { get; set; } = string.Empty;

    public string Sign { get; set; } = string.Empty;

    public MessageTemplateStatuses Status { get; set; }

    public MessageTemplateAuditStatuses AuditStatus { get; set; }

    public DateTimeOffset? AuditTime { get; set; }

    public DateTimeOffset? InvalidTime { get; set; }

    public string AuditReason { get; set; } = string.Empty;

    public int TemplateType { get; set; }

    public long PerDayLimit { get; set; }

    public virtual bool IsStatic { get; set; }

    public List<MessageTemplateItemQueryModel> Items { get; set; } = new();

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
