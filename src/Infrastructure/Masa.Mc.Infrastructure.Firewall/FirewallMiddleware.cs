// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public class FirewallMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFirewallValidator _validator;
    private readonly ILogger<FirewallMiddleware> _logger;

    public FirewallMiddleware(RequestDelegate next,
        IFirewallValidator validator,
        ILogger<FirewallMiddleware> logger)
    {
        _next = next;
        _validator = validator;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method != HttpMethod.Get.Method)
        {
            string path = context.Request.Path.ToString().ToLower();
            if (_validator.ValidateUrl(path))
            {
                await _next.Invoke(context);
                return;
            }
            string ip = FirewallMiddlewareExtensions.GetClientIp(context);
            if (!_validator.ValidateIp(ip))
            {
                _logger.LogInformation($"The ip {ip} invalid.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
        
        await _next.Invoke(context);
    }
}