// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Utils;

public static class DateTimeParseUtil
{
    private static readonly Lazy<TimeZoneInfo> ChinaTimeZone = new(() => TimeZoneUtil.FindTimeZoneById("China Standard Time"));

    public static DateTimeOffset ParseLocalToUtcOrNow(string? value, string format)
    {
        return ParseInTimeZoneToUtcOrNow(value, format, TimeZoneInfo.Local);
    }

    public static DateTimeOffset ParseChinaTimeToUtcOrNow(string? value, string format)
    {
        return ParseInTimeZoneToUtcOrNow(value, format, ChinaTimeZone.Value);
    }

    public static DateTimeOffset ParseInTimeZoneToUtcOrNow(string? value, string format, TimeZoneInfo timeZoneInfo)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
        {
            // Provider callback sendTime is a local time string without offset.
            // Convert it with an explicit timezone to avoid host timezone dependency.
            var unspecifiedTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Unspecified);
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(unspecifiedTime, timeZoneInfo);
            return new DateTimeOffset(utcTime, TimeSpan.Zero);
        }

        return DateTimeOffset.UtcNow;
    }
}
