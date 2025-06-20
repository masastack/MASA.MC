// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public class XiaomiPushOptions : IXiaomiPushOptions
{
    public string AppSecret { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public string CallbackUrl { get; set; } = string.Empty;
}
