// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public interface IOppoPushOptions : IOptions
{
    public string AppKey { get; set; }

    public string MasterSecret { get; set; }

    public string CallbackUrl { get; set; }
}
