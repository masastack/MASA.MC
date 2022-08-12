// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Middleware;

public class AdminSafeListMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminSafeListMiddleware> _logger;
    private readonly WhiteListOptions _whiteListOptions;


    public AdminSafeListMiddleware(
        RequestDelegate next,
        ILogger<AdminSafeListMiddleware> logger,
        WhiteListOptions whiteListOptions)
    {
        _whiteListOptions = whiteListOptions;
        _next = next;
        _logger = logger;
    }


    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method != HttpMethod.Get.Method)
        {
            string path = context.Request.Path.ToString().ToLower();
            if (ValidateUrl(path))
            {
                await _next.Invoke(context);
                return;
            }
            _logger.LogDebug("Request from Remote path: {path}", path);
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            
            if (!ValidateIp(remoteIp))
            {
                _logger.LogWarning(
                    "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next.Invoke(context);
    }



    public bool ValidateUrl(string url)
    {
        string valUrl = url;
        if (valUrl.Length > 1 && valUrl.Last() == '/')
        {
            valUrl = valUrl.Substring(0, valUrl.Length - 1);
        }

        foreach (var item in _whiteListOptions.UrlWhiteList)
        {
            if (Regex.IsMatch(valUrl, item, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public bool ValidateIp(string ip)
    {
        if (_whiteListOptions.IpWhiteList.Any(x => x == "0.0.0.0"))
        {
            return true;
        }

        foreach (var item in _whiteListOptions.IpWhiteList)
        {
            if (Regex.IsMatch(ip, item, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}