// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskHistoryDto : AuditEntityDto<Guid, Guid>
{
    public Guid MessageTaskId { get; set; }

    public string TaskHistoryNo { get; set; } = string.Empty;

    public MessageTaskDto MessageTask { get; set; } = new();

    public ReceiverTypes ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; }

    public MessageTaskHistoryStatuses Status { get; set; }

    public List<MessageTaskReceiverDto> Receivers { get; set; } = new();

    public SendRuleDto SendRules { get; set; } = new();

    public DateTimeOffset? SendTime { get; set; }

    public DateTimeOffset? CompletionTime { get; set; }

    public DateTimeOffset? WithdrawTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
