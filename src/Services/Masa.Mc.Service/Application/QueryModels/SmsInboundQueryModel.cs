// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class SmsInboundQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid ChannelId { get; set; }

    public string Mobile { get; set; } = string.Empty;

    public string SmsContent { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public string AddSerial { get; set; } = string.Empty;

    public SmsInboundProviders Provider { get; set; }

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
