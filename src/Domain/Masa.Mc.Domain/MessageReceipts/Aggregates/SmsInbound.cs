// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageReceipts.Aggregates;

public class SmsInbound : FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; private set; }

    public string Mobile { get; private set; } = string.Empty;

    public string SmsContent { get; private set; } = string.Empty;

    public DateTimeOffset SendTime { get; private set; }

    public string AddSerial { get; private set; } = string.Empty;

    public SmsInboundProviders Provider { get; private set; }

    public SmsInbound(Guid channelId, string mobile, string smsContent, DateTimeOffset sendTime, string addSerial, SmsInboundProviders provider)
    {
        ChannelId = channelId;
        Mobile = mobile;
        SmsContent = smsContent;
        SendTime = sendTime;
        AddSerial = addSerial;
        Provider = provider;
    }
}
