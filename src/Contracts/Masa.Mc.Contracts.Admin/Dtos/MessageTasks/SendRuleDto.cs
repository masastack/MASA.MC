// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendRuleDto
{
    public bool IsCustom { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public long SendingCount { get; set; }
}
