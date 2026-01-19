// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public record GetAppVendorSendStatisticsQuery(ChannelSendStatisticsInputDto Input) : Query<List<AppVendorSendStatisticsDto>>
{
    public override List<AppVendorSendStatisticsDto> Result { get; set; } = new();
}
