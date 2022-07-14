// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskSendingRule
{
    public bool IsCustom { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public long SendingCount { get; set; }
}
