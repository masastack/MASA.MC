// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics;

[ExcelExporter(AutoFitAllColumn = false)]
public sealed class FailureReasonDetailExportItem
{
    [ExporterHeader(DisplayName = "消息标题", IsAutoFit = false)]
    public string DisplayName { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "渠道用户标识", IsAutoFit = false)]
    public string ChannelUserIdentity { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "失败原因", IsAutoFit = false)]
    public string FailureReason { get; init; } = string.Empty;

    [ExporterHeader(DisplayName = "预计发送时间", Format = "yyyy-MM-dd HH:mm:ss", IsAutoFit = false)]
    public DateTime? ExpectSendTime { get; init; }

    [ExporterHeader(DisplayName = "实际发送时间", Format = "yyyy-MM-dd HH:mm:ss", IsAutoFit = false)]
    public DateTime? SendTime { get; init; }

    [ExporterHeader(DisplayName = "消息ID", IsAutoFit = false)]
    public string MessageId { get; init; } = string.Empty;
}
