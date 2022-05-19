// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskDto : AuditEntityDto<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public ChannelDto Channel { get; set; } = new();

    public MessageEntityTypes EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public ReceiverTypes ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; }

    public DateTime? SendTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public MessageInfoDto MessageInfo { get; set; } = new();

    public List<MessageTaskReceiverDto> Receivers { get; set; } = new();

    public SendRuleDto SendRules { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
