// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class MessageRecordQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid UserId { get; set; }

    public Guid? ChannelId { get; set; }

    public ChannelQueryModel Channel { get; set; } = default!;

    public Guid MessageTaskId { get; set; }

    public MessageTaskQueryModel MessageTask { get; set; } = new();

    public Guid MessageTaskHistoryId { get; set; }

    public bool? Success { get; set; }

    public DateTimeOffset? SendTime { get; set; }

    public DateTimeOffset? ExpectSendTime { get; set; }

    public string FailureReason { get; set; } = string.Empty;

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public string DisplayName { get; set; } = string.Empty;

    public MessageEntityTypes MessageEntityType { get; set; }

    public Guid MessageEntityId { get; set; }

    public string ChannelUserIdentity { get; protected set; } = string.Empty;

    public string SystemId { get; set; } = string.Empty;

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
