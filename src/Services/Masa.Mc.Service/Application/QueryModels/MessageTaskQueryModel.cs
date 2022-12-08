// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class MessageTaskQueryModel : Entity<Guid>, ISoftDelete
{
    public string DisplayName { get; set; } = string.Empty;

    public ChannelTypes? ChannelType { get; set; }

    public Guid? ChannelId { get; set; }

    public ChannelQueryModel Channel { get; set; } = default!;

    public MessageEntityTypes EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public ReceiverTypes ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; }

    public DateTimeOffset? SendTime { get; set; }

    public DateTimeOffset? ExpectSendTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public List<MessageTaskReceiver> Receivers { get; set; } = new();

    public MessageTaskSendingRule SendRules { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public MessageTaskStatuses Status { get; set; }

    public MessageTaskSources Source { get; set; }

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}

