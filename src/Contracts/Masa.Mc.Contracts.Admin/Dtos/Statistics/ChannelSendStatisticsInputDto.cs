// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics;

public class ChannelSendStatisticsInputDto
{
    public Guid? ChannelId { get; set; }

    public Guid? TemplateId { get; set; }

    public AppVendor? Vendor { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
