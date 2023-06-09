// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public class SendPushSuccessResponse
{
    [JsonProperty("sendno")]
    public string Sendno { get; set; } = string.Empty;

    [JsonProperty("msg_id")]
    public string MsgId { get; set; } = string.Empty;
}
