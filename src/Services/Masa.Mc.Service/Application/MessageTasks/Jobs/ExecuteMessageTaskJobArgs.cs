// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ExecuteMessageTaskJobArgs
{
    public Guid MessageTaskId { get; set; }

    public bool IsTest { get; set; } = false;

    public Guid JobId { get; set; } = default;

    public Guid TaskId { get; set; } = default;
}
