// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public interface IXiaomiPushOptions : IOptions
{
    public string AppSecret { get; set; }

    public string PackageName { get; set; }
}
