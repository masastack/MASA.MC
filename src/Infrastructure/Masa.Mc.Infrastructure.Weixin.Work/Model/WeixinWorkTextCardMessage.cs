// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Model;

public class WeixinWorkTextCardMessage : WeixinWorkMessageBase
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }

    public WeixinWorkTextCardMessage(string toUser, string title, string description, string url) : base(toUser, "textcard")
    {
        Title = title;
        Description = description;
        Url = url;
    }
}
