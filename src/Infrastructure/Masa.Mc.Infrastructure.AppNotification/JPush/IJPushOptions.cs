// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public interface IJPushOptions : IOptions
{
    public string AppKey { get; set; }

    public string MasterSecret { get; set; }
}
