// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public class JPushSendPushModel
{
    [JsonProperty("msg_id")]
    public string MsgId { get; set; } = string.Empty;

    [JsonProperty("error")]
    public JPushSendPushErrorModel? Error { get; set; }
}


public class JPushSendPushErrorModel
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("code")]
    public int Code { get; set; }
}
