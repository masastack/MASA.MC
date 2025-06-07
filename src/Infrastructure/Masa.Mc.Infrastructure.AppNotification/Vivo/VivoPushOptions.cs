// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Vivo;

public class VivoPushOptions: IVivoPushOptions
{
    public string AppId { get; set; } = string.Empty;

    public string AppKey { get; set; } = string.Empty;

    public string AppSecret { get; set; } = string.Empty;
}
