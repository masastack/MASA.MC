// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public class YunMasOptions: IYunMasOptions
{
    public string ApiUrl { get; set; } = string.Empty;
    public string EcName { get; set; } = string.Empty;
    public string ApId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public string AddSerial { get; set; } = "";
}
