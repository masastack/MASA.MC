// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class HuaweiReceiptInput
{
    public List<HuaweiReceiptStatusDto> Statuses { get; set; } = new();
}

public class HuaweiReceiptStatusDto
{
    public string BiTag { get; set; }

    public string AppPackageName { get; set; }

    public string Token { get; set; }

    public string RequestId { get; set; }
    
    public HuaweiDeliveryStatusDto DeliveryStatus { get; set; }
}

public class HuaweiDeliveryStatusDto
{
    public HuaweiDeliveryResult Result { get; set; }
    public long Timestamp { get; set; }
}

public enum HuaweiDeliveryResult
{
    [Description("成功送达")]
    Success = 0,

    [Description("应用未安装")]
    AppNotInstalled = 2,

    [Description("Token不存在")]
    TokenNotExist = 5,

    [Description("通知栏消息不展示")]
    NotificationNotShow = 6,

    [Description("非活跃设备")]
    InactiveDevice = 10,

    [Description("其它错误")]
    OtherError = 14,

    [Description("离线用户消息管控")]
    OfflineUserMessageControl = 15,

    [Description("userID不匹配")]
    UserIdNotMatch = 22,

    [Description("应用进程不存在，透传消息被缓存")]
    AppProcessNotExist = 27,

    [Description("系统版本或应用不支持该消息")]
    NotSupported = 31,

    [Description("设备处于开机未解锁状态")]
    DeviceNotUnlocked = 51,

    [Description("消息频控丢弃")]
    MessageRateLimited = 102,

    [Description("profileId不存在")]
    ProfileIdNotExist = 144,

    [Description("消息发送管控")]
    MessageControlled = 201,

    [Description("资讯营销类消息频次限制")]
    MarketingMessageLimited = 256
}