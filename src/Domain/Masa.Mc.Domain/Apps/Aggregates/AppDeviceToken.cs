// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Apps.Aggregates;

public class AppDeviceToken : FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; private set; }

    public Guid UserId { get; private set; }

    public string DeviceToken { get; private set; } = string.Empty;

    public AppDeviceTokenPlatform Platform { get; private set; }

    public DateTimeOffset RegisteredTime { get; private set; }

    public ExtraPropertyDictionary ExtraProperties { get; private set; } = new();

    public AppDeviceToken(Guid channelId, Guid userId, string deviceToken, AppDeviceTokenPlatform platform, DateTimeOffset registeredTime, ExtraPropertyDictionary extraProperties)
    {
        ChannelId = channelId;
        UserId = userId;
        DeviceToken = deviceToken;
        Platform = platform;
        RegisteredTime = registeredTime;
        ExtraProperties = extraProperties;
    }

    public void UpdateToken(string newToken, AppDeviceTokenPlatform platform)
    {
        DeviceToken = newToken;
        RegisteredTime = DateTimeOffset.UtcNow;
        Platform = platform;
    }
}
