// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Aggregates;

public class MessageTaskSendingRule : ValueObject
{
    public bool IsCustom { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public long SendingCount { get; set; }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return IsCustom;
        yield return CronExpression;
        yield return SendingCount;
    }

    public MessageTaskSendingRule() { }

    public MessageTaskSendingRule(bool isCustom, string cronExpression, long sendingCount)
    {
        IsCustom = isCustom;
        CronExpression = cronExpression;
        SendingCount = sendingCount;
    }
}
