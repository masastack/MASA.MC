// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Masa.Mc.Web.Admin.Global;

public class MyI18n: I18n
{
    private const string CultureCookieKey = "Masa_I18nConfig_Culture";

    private readonly CookieStorage _cookieStorage;

    public MyI18n(CookieStorage cookieStorage, IHttpContextAccessor httpContextAccessor) : base(cookieStorage, httpContextAccessor)
    {
        _cookieStorage = cookieStorage;
        string cultureName;
        if (httpContextAccessor.HttpContext is not null)
        {
            cultureName = httpContextAccessor.HttpContext.Request.Cookies[CultureCookieKey];
            Console.WriteLine("httpContextAccessor.HttpContext.Request.Cookies:" + cultureName);
            Console.WriteLine("CookieStorage:" + _cookieStorage.GetCookie(CultureCookieKey));
            if (cultureName is null)
            {
                var acceptLanguage = httpContextAccessor.HttpContext.Request.Headers["accept-language"].FirstOrDefault();
            }
        }
        else
        {
            cultureName = _cookieStorage.GetCookie(CultureCookieKey) ?? "";
            Console.WriteLine("cultureName:" + cultureName);
        }

        //var culture = GetValidCulture(cultureName);
    }
}
