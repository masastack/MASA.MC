// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Shared.MessageTasks;

public enum MessageTaskStatuses
{
    WaitSend = 1,
    Sending = 2,
    Cancel = 3,
    Success = 4,
    Fail = 5,
    PartialFailure = 6
}
