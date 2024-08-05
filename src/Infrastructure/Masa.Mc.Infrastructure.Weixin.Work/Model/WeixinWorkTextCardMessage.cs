// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model;

public class WeixinWorkTextCardMessage : WeixinWorkMessageBase
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public WeixinWorkTextCardMessage(List<string> userId, string title, string description, string url) : base(userId, "textcard")
    {
        Title = title;
        Description = description;
        Url = url;
    }
}
