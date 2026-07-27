// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Extensions;

public static class CultureTimeZoneResolver
{
    private static readonly IReadOnlyDictionary<string, string> RegionTimeZoneIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["CN"] = "China Standard Time"
    };

    public static TimeZoneInfo GetDefaultCultureTimeZone()
    {
        var culture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture
            ?? System.Globalization.CultureInfo.DefaultThreadCurrentUICulture
            ?? System.Globalization.CultureInfo.CurrentCulture;

        return GetCultureTimeZone(culture);
    }

    public static DateTime? ConvertToDefaultCultureTime(DateTimeOffset? time)
    {
        return ConvertTime(time, GetDefaultCultureTimeZone());
    }

    public static DateTime? ConvertTime(DateTimeOffset? time, TimeZoneInfo timeZone)
    {
        if (!time.HasValue)
        {
            return null;
        }

        return TimeZoneInfo.ConvertTime(time.Value, timeZone).DateTime;
    }

    private static TimeZoneInfo GetCultureTimeZone(System.Globalization.CultureInfo culture)
    {
        if (TryGetRegionName(culture, out var regionName) &&
            RegionTimeZoneIds.TryGetValue(regionName, out var timeZoneId))
        {
            return TimeZoneUtil.FindTimeZoneById(timeZoneId);
        }

        return TimeZoneInfo.Utc;
    }

    private static bool TryGetRegionName(System.Globalization.CultureInfo culture, out string regionName)
    {
        regionName = string.Empty;

        if (culture.IsNeutralCulture || string.IsNullOrWhiteSpace(culture.Name))
        {
            return false;
        }

        try
        {
            regionName = new System.Globalization.RegionInfo(culture.Name).TwoLetterISORegionName;
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

}
