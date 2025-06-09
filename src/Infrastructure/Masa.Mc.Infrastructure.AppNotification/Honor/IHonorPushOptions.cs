// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Honor;

public interface IHonorPushOptions : IOptions
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string AppId { get; set; }
}
