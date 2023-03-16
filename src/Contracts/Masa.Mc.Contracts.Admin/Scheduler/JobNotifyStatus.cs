// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Scheduler;

public enum JobNotifyStatus
{
    Failure = 1,
    Timeout,
    Enabled,
    Disable,
    Delete
}