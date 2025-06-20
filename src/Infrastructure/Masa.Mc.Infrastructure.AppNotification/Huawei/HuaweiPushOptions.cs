// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiPushOptions : IHuaweiPushOptions
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string ProjectId { get; set; } = string.Empty;

    public string CallbackId { get; set; } = string.Empty;
}
