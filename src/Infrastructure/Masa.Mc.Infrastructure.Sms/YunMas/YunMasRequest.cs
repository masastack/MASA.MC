// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

using Newtonsoft.Json;

public class YunMasRequest
{
    [JsonProperty(Order = 1)]
    public string ecName { get; set; }
    [JsonProperty(Order = 2)]
    public string apId { get; set; }
    [JsonProperty(Order = 3)]
    public string mobiles { get; set; }
    [JsonProperty(Order = 4)]
    public string content { get; set; }
    [JsonProperty(Order = 5)]
    public string sign { get; set; }
    [JsonProperty(Order = 6)]
    public string addSerial { get; set; }
    [JsonProperty(Order = 7)]
    public string mac { get; set; }
}
