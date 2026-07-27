// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Model.Response;

public class WeixinMiniProgramMessageResponse : WxJsonResult
{
    public int ErrCode => ErrorCodeValue;

    public string ErrMsg => errmsg ?? string.Empty;

    public string msgid { get; set; } = string.Empty;

    public string MsgId => msgid;
}
