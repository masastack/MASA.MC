// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public class DefaultFirewallValidator: IFirewallValidator
{
    public FirewallOptions Options { get; set; }

    public DefaultFirewallValidator(FirewallOptions options)
    {
        Options = options;

    }
    public bool ValidateUrl(string url)
    {
        string valUrl = url;
        if (valUrl.Length > 1 && valUrl.Last() == '/')
        {
            valUrl = valUrl.Substring(0, valUrl.Length - 1);
        }

        string[] urlWhiteList = Options.UrlWhiteList.Split(';');
        foreach (var item in urlWhiteList)
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
        string[] ipWhiteList = Options.IpWhiteList.Split(';');

        if (ipWhiteList.Any(x => x == "0.0.0.0"))
        {
            return true;
        }

        foreach (var item in ipWhiteList)
        {
            if (Regex.IsMatch(ip, item, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
