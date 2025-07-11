﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Apps.Commands;

public record AppNotificationSubscribeCommand(Guid ChannelId, AppPlatform Platform, string DeviceToken) : Command;
