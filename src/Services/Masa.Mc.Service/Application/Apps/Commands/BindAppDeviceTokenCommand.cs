﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Apps.Commands;

public record BindAppDeviceTokenCommand(Guid ChannelId, string DeviceToken, AppPlatform Platform) : Command
{
}
