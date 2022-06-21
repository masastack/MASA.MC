// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Enums.MessageTasks;

public enum MessageTaskHistoryStatuses
{
    WaitSend = 1,
    Sending,
    Completed,
    Withdrawn,
    Success = 101,
    Fail = 102,
    PartialFailure = 103
}
