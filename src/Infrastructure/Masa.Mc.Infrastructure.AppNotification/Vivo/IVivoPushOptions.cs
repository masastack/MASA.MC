// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Vivo;

public interface IVivoPushOptions : IOptions
{
    public string AppId { get; set; }

    public string AppKey { get; set; }

    public string AppSecret { get; set; }

    public string CallbackId { get; set; }
}
