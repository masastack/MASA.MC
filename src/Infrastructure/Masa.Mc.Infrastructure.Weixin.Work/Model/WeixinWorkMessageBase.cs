// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model;

public class WeixinWorkMessageBase
{
    public List<string> UserId { get; set; }

    public string MsgType { get; set; }

    public WeixinWorkMessageBase(List<string> userId, string msgType)
    {
        UserId = userId;
        MsgType = msgType;
    }
}
