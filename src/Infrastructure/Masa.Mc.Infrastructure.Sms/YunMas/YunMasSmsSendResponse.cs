// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public class YunMasSmsSendResponse
{
    [JsonPropertyName("rspcod")]
    public string Rspcod { get; set; }

    [JsonPropertyName("msgGroup")]
    public string MgsGroup { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }
}
