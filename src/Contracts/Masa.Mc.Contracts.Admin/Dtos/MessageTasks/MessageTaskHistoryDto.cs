// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageTaskHistoryDto : AuditEntityDto<Guid, Guid>
{
    public Guid MessageTaskId { get; set; }

    public string TaskHistoryNo { get; set; } = string.Empty;

    public MessageTaskDto MessageTask { get; set; } = new();

    public MessageTaskHistoryStatuses Status { get; set; }

    public DateTimeOffset? SendTime { get; set; }

    public DateTimeOffset? CompletionTime { get; set; }

    public DateTimeOffset? WithdrawTime { get; set; }
}
