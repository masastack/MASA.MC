// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public static class FirewallMiddlewareExtensions
{
    public static IApplicationBuilder UseFirewall(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FirewallMiddleware>();
    }

    public static string GetClientIp(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }
        return ip;
    }
}