// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model.Response;

public class WeixinWorkMessageResponseBase
{
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;
    [JsonPropertyName("invaliduser")]
    public string InvalidUser { get; set; } = string.Empty;
    [JsonPropertyName("invalidparty")]
    public string InvalidParty { get; set; } = string.Empty;
    [JsonPropertyName("invalidtag")]
    public string InvalidTag { get; set; } = string.Empty;
    [JsonPropertyName("unlicenseduser")]
    public string UnlicensedUser { get; set; } = string.Empty;
    [JsonPropertyName("msgid")]
    public string MsgId { get; set; } = string.Empty;
    [JsonPropertyName("response_code")]
    public string ResponseCode { get; set; } = string.Empty;
}
