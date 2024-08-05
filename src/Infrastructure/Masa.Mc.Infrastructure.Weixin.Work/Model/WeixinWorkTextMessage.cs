// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model;

public class WeixinWorkTextMessage : WeixinWorkMessageBase
{
    public string Content { get; set; }
    public WeixinWorkTextMessage(List<string> userId, string content) : base(userId, "text")
    {
        Content = content;
    }
}
