// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public interface IYunMasOptions : IOptions
{
    public string ApiUrl { get; set; }
    public string EcName { get; set; }
    public string ApId { get; set; }
    public string SecretKey { get; set; }
    public string Sign { get; set; }
    public string AddSerial { get; set; }
}