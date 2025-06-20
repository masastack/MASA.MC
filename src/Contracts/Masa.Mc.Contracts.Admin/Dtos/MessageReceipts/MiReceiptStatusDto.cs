// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class MiReceiptStatusDto
{
    public string? Param { get; set; }
    public MiReceiptType Type { get; set; }
    public string Targets { get; set; } = string.Empty;
    public string? JobKey { get; set; }
    public string? BarStatus { get; set; }
    public long Timestamp { get; set; }
    public int? ErrorCode { get; set; }
    public Dictionary<string, string>? ReplaceTarget { get; set; }
    public Dictionary<string, string>? Extra { get; set; }
}

public enum MiReceiptType
{
    [Description("消息送达")]
    Delivered = 1,
    [Description("消息点击")]
    Clicked = 2,
    [Description("目标设备无效")]
    InvalidTarget = 16,
    [Description("禁用推送")]
    PushDisabled = 32,
    [Description("目标设备不符合过滤条件")]
    NotMatchFilter = 64,
    [Description("当日推送总量超限")]
    OverQuota = 128,
    [Description("消息有效期TTL过期")]
    Expired = 1024
}