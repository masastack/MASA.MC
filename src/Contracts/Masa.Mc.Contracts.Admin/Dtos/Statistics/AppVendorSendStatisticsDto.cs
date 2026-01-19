// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Statistics;

public class AppVendorSendStatisticsDto : ChannelSendStatisticsDto
{
    public AppVendor? Vendor { get; set; }

    public string VendorName { get; set; } = string.Empty;
}
