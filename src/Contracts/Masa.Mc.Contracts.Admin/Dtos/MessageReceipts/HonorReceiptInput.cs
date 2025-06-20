// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class HonorReceiptInput
{
    public List<HonorReceiptStatusDto> Statuses { get; set; } = new();
}

public class HonorReceiptStatusDto
{
    public string Token { get; set; }
    public string BiTag { get; set; }
    public HonorReceiptStatus Status { get; set; }
    public long Timestamp { get; set; }
    public string Appid { get; set; }
    public string RequestId { get; set; }
}

public enum HonorReceiptStatus
{
    [Description("透传消息已成功下发给目标应用进程")]
    PassThroughDelivered = 40000001,

    [Description("通知栏消息投递成功（已过滤token失效，通知开关关闭等情况），准备在通知栏展示")]
    Success = 40000002,

    [Description("目标应用不存在（未安装）")]
    AppNotInstalled = 40000003,

    [Description("指定的Token在终端用户下失效或不存在")]
    TokenInvalidOrNotExist = 40000004,

    [Description("目标应用通知栏开关被关闭，无通知栏展示权限")]
    NotificationSwitchOff = 40000005,

    [Description("目标应用在终端设备被禁用或停用")]
    AppDisabledOrStopped = 40000006,

    [Description("通知栏消息被点击")]
    NotificationClicked = 40000007,

    [Description("消息因系统原因丢弃，展示失败。主要的原因可能是消息发送数量超过配额")]
    SystemDiscarded = 40000009,

    [Description("通知栏跳转目标应用页面失败。一般由于指定的目标页面不存在或者无权限打开该页面")]
    NotificationJumpFailed = 40000011,

    [Description("通知消息被管控，表示手机处于息屏状态一段时间后，系统对电量优化的一种管控状态，当手机再次亮屏后，会重新下发消息")]
    NotificationControlled = 40000013,

    [Description("token指定的用户与终端设备当前用户不匹配")]
    UserNotMatch = 40000014,

    [Description("通知栏展示失败，消息丢弃。（系统原因，通常是Intent校验不通过，超过通知栏展示上限等）")]
    NotificationShowFailed = 40000015,

    [Description("在终端设备上目标应用进程不存在导致透传消息被缓存")]
    AppProcessNotExist = 50000003
}