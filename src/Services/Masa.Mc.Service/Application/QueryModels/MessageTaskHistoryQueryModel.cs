// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class MessageTaskHistoryQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid MessageTaskId { get; set; }

    public string TaskHistoryNo { get; set; } = string.Empty;

    public MessageTaskQueryModel MessageTask { get; set; } = default!;

    public MessageTaskHistoryStatuses Status { get; set; }

    public DateTimeOffset? SendTime { get; set; }

    public DateTimeOffset? CompletionTime { get; set; }

    public DateTimeOffset? WithdrawTime { get; set; }

    public bool IsTest { get; set; }

    public Guid SchedulerTaskId { get; set; }

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
