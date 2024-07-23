// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model;

public class WeixinWorkMessageBase
{
    [JsonPropertyName("touser")]
    public string ToUser { get; set; }
    [JsonPropertyName("toparty")]
    public string ToParty { get; set; } = string.Empty;
    [JsonPropertyName("totag")]
    public string ToTag { get; set; } = string.Empty;
    [JsonPropertyName("msgtype")]
    public string MsgType { get; set; }

    public WeixinWorkMessageBase(string toUser, string msgType)
    {
        ToUser = toUser;
        MsgType = msgType;
    }
}
