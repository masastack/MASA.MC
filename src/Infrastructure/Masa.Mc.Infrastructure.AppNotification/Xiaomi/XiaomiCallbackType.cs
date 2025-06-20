// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public enum XiaomiCallbackType
{
    Delivered = 1,
    Clicked = 2,
    InvalidDevice = 16,
    PushDisabled = 32,
    FilterMismatch = 64,
    PushLimitExceeded = 128,
    TTLEnded = 1024
}
