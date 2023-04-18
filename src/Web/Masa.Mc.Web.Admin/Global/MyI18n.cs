// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

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
            cultureName = httpContextAccessor.HttpContext.Request.Cookies[CultureCookieKey]??"";
            Console.WriteLine("httpContextAccessor.HttpContext.Request.Cookies:" + cultureName);
            Console.WriteLine("CookieStorage:" + _cookieStorage.GetCookie(CultureCookieKey));
            //if (cultureName is null)
            //{
            //    var acceptLanguage = httpContextAccessor.HttpContext.Request.Headers["accept-language"].FirstOrDefault();
            //    if (acceptLanguage is not null)
            //    {
            //        cultureName = acceptLanguage
            //                      .Split(",")
            //                      .Select(lang =>
            //                      {
            //                          var arr = lang.Split(';');
            //                          if (arr.Length == 1) return (arr[0], 1);
            //                          else return (arr[0], Convert.ToDecimal(arr[1].Split("=")[1]));
            //                      })
            //                      .OrderByDescending(lang => lang.Item2)
            //                      .FirstOrDefault(lang => I18nCache.ContainsCulture(lang.Item1)).Item1;
            //    }
            //}
        }
        else
        {
            cultureName = _cookieStorage.GetCookie(CultureCookieKey) ?? "";
            Console.WriteLine("cultureName:" + cultureName);
        }
        Console.WriteLine("StartGetValidCulture:" + cultureName);
        CultureInfo validCulture = GetValidCulture(cultureName);
        //SetCulture(validCulture);
    }

    private static CultureInfo GetValidCulture(string cultureName)
    {
        CultureInfo cultureInfo;
        try
        {
            cultureInfo = CultureInfo.CreateSpecificCulture(cultureName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetValidCulture：ex:{ex.Message}");
            cultureInfo = CultureInfo.CurrentUICulture;
        }
        Console.WriteLine($"GetValidCulture：{cultureName}: {cultureInfo.Name}"); 
        if (cultureInfo.Name == string.Empty)
        {
            //cultureInfo = I18nCache.DefaultCulture;
        }

        string name = cultureInfo.Name;
        if (1 == 0)
        {
        }

        CultureInfo result = ((name == "zh-Hans-CN") ? new CultureInfo("zh-CN") : ((!(name == "zh-Hant-CN")) ? cultureInfo : new CultureInfo("zh-TW")));
        if (1 == 0)
        {
        }

        return result;
    }
}
