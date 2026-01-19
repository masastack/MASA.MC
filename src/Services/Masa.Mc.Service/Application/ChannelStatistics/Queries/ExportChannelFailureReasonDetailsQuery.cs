// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ChannelStatistics.Queries;

public record ExportChannelFailureReasonDetailsQuery(ChannelSendStatisticsInputDto Input) : Query<byte[]>
{
    public override byte[] Result { get; set; } = Array.Empty<byte>();
}
