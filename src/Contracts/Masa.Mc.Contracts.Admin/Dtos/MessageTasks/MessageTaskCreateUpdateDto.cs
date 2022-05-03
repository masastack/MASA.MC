// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskCreateUpdateDto
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public MessageEntityType EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public ReceiverType ReceiverType { get; set; }

    public DateTime? SendTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public ReceiverDto Receivers { get; set; } = new();

    public SendingRuleDto SendingRules { get; set; } = new();

    public MessageInfoCreateUpdateDto MessageInfo { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();
}