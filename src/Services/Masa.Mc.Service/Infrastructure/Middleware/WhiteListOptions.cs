// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Middleware;

public class WhiteListOptions
{
    public List<string> IpWhiteList { get; set; } = new();

    public List<string> UrlWhiteList { get; set; } = new();
}
