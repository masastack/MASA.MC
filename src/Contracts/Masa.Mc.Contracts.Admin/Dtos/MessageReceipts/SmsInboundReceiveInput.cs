// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageReceipts;

public class SmsInboundReceiveInput
{
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [JsonPropertyName("smsContent")]
    public string SmsContent { get; set; } = string.Empty;

    [JsonPropertyName("sendTime")]
    public string SendTime { get; set; } = string.Empty;

    [JsonPropertyName("addSerial")]
    public string AddSerial { get; set; } = string.Empty;
}
