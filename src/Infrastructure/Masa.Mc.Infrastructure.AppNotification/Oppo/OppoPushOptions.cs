// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public class OppoPushOptions: IOppoPushOptions
{
    public string AppKey { get; set; } = string.Empty;

    public string MasterSecret { get; set; } = string.Empty;

    public string CallbackUrl { get; set; } = string.Empty;
}
