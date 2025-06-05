// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public interface IHuaweiPushOptions : IOptions
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string ProjectId { get; set; }
}
