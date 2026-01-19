// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public record GetChannelFailureReasonOverviewQuery(ChannelSendStatisticsInputDto Input) : Query<List<ChannelFailureReasonOverviewDto>>
{
    public override List<ChannelFailureReasonOverviewDto> Result { get; set; } = new();
}
