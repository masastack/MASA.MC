// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class AppDeviceTokenQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid ChannelId { get; set; }

    public Guid UserId { get; set; }

    public string DeviceToken { get; set; } = string.Empty;

    public AppDeviceTokenPlatform Platform { get; set; }

    public DateTimeOffset RegisteredTime { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    public bool IsDeleted { get; set; }
}
