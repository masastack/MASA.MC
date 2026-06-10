// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Extensions;

public static class CultureConfiguration
{
    public static void SetDefaultCulture(string cultureName)
    {
        var defaultCulture = System.Globalization.CultureInfo.GetCultureInfo(cultureName);
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
        System.Globalization.CultureInfo.CurrentCulture = defaultCulture;
        System.Globalization.CultureInfo.CurrentUICulture = defaultCulture;
    }
}
