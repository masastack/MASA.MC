// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public class FirewallOptions
{
    public string IpWhiteList { get; set; } = string.Empty;

    public string UrlWhiteList { get; set; } = string.Empty;
}
