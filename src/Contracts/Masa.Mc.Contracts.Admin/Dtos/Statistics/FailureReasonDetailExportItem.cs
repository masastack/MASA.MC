// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics;

public sealed class FailureReasonDetailExportItem
{
    [ExporterHeader(DisplayName = "消息标题")]
    public string DisplayName { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "渠道用户标识")]
    public string ChannelUserIdentity { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "失败原因")]
    public string FailureReason { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "预计发送时间")]
    public DateTimeOffset? ExpectSendTime { get; init; }

    [ExporterHeader(DisplayName = "实际发送时间")]
    public DateTimeOffset? SendTime { get; init; }

    [ExporterHeader(DisplayName = "消息ID")]
    public string MessageId { get; init; } = string.Empty;
}
