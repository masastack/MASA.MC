// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model.Response;

public class WeixinWorkMessageResponseBase
{
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;
}
