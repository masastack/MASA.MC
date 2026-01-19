// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics;

public class ChannelSendStatisticsDto
{
    public decimal SuccessRate { get; set; }

    public long TotalCount { get; set; }

    public long SuccessCount { get; set; }

    public long FailCount { get; set; }

    public long NoReceiptCount { get; set; }
}
