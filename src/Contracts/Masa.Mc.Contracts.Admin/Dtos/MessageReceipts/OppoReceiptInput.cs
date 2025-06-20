// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class OppoReceiptInput
{
    public string MessageId { get; set; }
    public string AppId { get; set; }
    public string TaskId { get; set; }
    public string RegistrationIds { get; set; }
    public long EventTime { get; set; }
    public string Param { get; set; }
    public string EventType { get; set; }
}


public enum OppoReceiptEventType
{
    [Description("消息成功到达设备")]
    PushArrive,

    [Description("无效RegistrationID")]
    RegidInvalid,

    [Description("单应用单设备限量")]
    UserDailyLimit
}