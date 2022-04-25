﻿namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendingRuleDto
{
    public long Timeout { get; set; }
    public long RetryInterval { get; set; }
    public int RetryCount{ get; set; }
    public bool IsSendingInterval { get; set; }
    public long SendingInterval { get; set; }
    public long SendingCount { get; set; }
    public bool IsTiming { get; set; }
    //public DateOnly? SendingDate { get; set; }
    //public TimeOnly? SendingTime { get; set; }
    public DateTime? SendTime { get; set; }

}
