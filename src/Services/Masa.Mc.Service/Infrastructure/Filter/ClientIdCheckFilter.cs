// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Filters;

namespace Masa.Mc.Service.Admin.Infrastructure.Filter;

public class ClientIdCheckFilter : ActionFilterAttribute
{
    private readonly ILogger _logger;
    private readonly string _safelist;


    public ClientIdCheckFilter(string safelist, ILogger logger)
    {
        _safelist = safelist;
        _logger = logger;
    }


    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
        _logger.LogDebug("Remote IpAddress: {RemoteIp}", remoteIp);
        var ip = _safelist.Split(';');
        var badIp = true;


        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }


        foreach (var address in ip)
        {
            var testIp = IPAddress.Parse(address);


            if (testIp.Equals(remoteIp))
            {
                badIp = false;
                break;
            }
        }


        if (badIp)
        {
            _logger.LogWarning("Forbidden Request from IP: {RemoteIp}", remoteIp);
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }


        base.OnActionExecuting(context);
    }
}