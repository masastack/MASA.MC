// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HmsPushRequest
{
    public bool ValidateOnly { get; set; }
    public HmsMessage Message { get; set; }
}

public class HmsMessage
{
    public string[] Token { get; set; }
    public string Topic { get; set; }
    public HmsNotification Notification { get; set; }
    public string Data { get; set; }
    public HmsAndroidConfig Android { get; set; }
}

public class HmsNotification
{
    public string Title { get; set; }
    public string Body { get; set; }
}

public class HmsAndroidConfig
{
    public int CollapseKey { get; set; } = -1;
    public string Urgency { get; set; } = "NORMAL";
    public string Category { get; set; }
    public string Ttl { get; set; } = "86400s";
    public HmsAndroidNotification Notification { get; set; }
    [JsonPropertyName("receipt_id")]
    public string ReceiptId { get; set; }
}

public class HmsAndroidNotification
{
    public string Icon { get; set; }
    public string Image { get; set; }
    public string Sound { get; set; }
    public bool DefaultSound { get; set; }
    public string Importance { get; set; } = "NORMAL";

    [JsonPropertyName("click_action")]
    public object ClickAction { get; set; }
}