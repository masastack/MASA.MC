// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Enums.MessageTasks;

public enum MessageTaskStatuses
{
    WaitSend = 1,
    Sending = 2,
    Success = 4,
    Fail = 5,
    PartialFailure = 6
}
