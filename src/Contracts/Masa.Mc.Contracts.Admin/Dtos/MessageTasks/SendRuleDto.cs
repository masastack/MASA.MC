// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendRuleDto
{
    public bool IsSendingInterval { get; set; }
    public long SendingInterval { get; set; }
    public long SendingCount { get; set; }
    public bool IsTiming { get; set; }
    public DateTimeOffset? SendTime { get; set; }
}
