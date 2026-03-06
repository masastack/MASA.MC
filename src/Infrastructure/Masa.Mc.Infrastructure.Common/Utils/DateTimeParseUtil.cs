// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Utils;

public static class DateTimeParseUtil
{
    public static DateTimeOffset ParseLocalToUtcOrNow(string? value, string format)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsedTime))
        {
            return parsedTime.ToUniversalTime();
        }

        return DateTimeOffset.UtcNow;
    }
}
