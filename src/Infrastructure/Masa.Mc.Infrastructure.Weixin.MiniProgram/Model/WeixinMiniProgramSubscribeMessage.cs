// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Model;

public class WeixinMiniProgramSubscribeMessage
{
    public string ToUser { get; }

    public string TemplateId { get; }

    public string Page { get; }

    public Dictionary<string, string> Data { get; }

    public WeixinMiniProgramSubscribeMessage(string toUser, string templateId, string page, Dictionary<string, string> data)
    {
        ToUser = toUser;
        TemplateId = templateId;
        Page = page;
        Data = data;
    }
}
