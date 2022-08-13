// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public interface IFirewallValidator
{
    FirewallOptions Options { get; set; }

    bool ValidateUrl(string url);

    bool ValidateIp(string ip);
}
