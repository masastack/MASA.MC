// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public record GetChannelSendTrendQuery(ChannelSendStatisticsInputDto Input) : Query<List<ChannelSendTrendDto>>
{
    public override List<ChannelSendTrendDto> Result { get; set; } = new();
}
