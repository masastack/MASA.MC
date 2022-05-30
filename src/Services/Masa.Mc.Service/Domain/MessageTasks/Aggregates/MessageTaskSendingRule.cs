// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskSendingRule
{
    public bool IsSendingInterval { get; protected set; }
    public long SendingInterval { get; protected set; }
    public long SendingCount { get; protected set; }
    public bool IsTiming { get; protected set; }
    public DateTimeOffset? SendTime { get; protected set; }
}
