// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class SmsInboundDto : AuditEntityDto<Guid, Guid>
{
    public Guid ChannelId { get; set; }

    public string Mobile { get; set; } = string.Empty;

    public string SmsContent { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public string AddSerial { get; set; } = string.Empty;

    public SmsInboundProviders Provider { get; set; }
}
