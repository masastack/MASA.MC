﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Model;

public class AppMessageOptions
{
    public bool IsWebsiteMessage { get; set; }

    public string IntentUrl { get; set; } = string.Empty;

    public bool IsApnsProduction { get; set; }
}
