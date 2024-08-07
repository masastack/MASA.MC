﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResolveMessageTaskJobArgs : MultiEnvironment
{
    public Guid MessageTaskId { get; set; }

    public Guid OperatorId { get; set; } = default;

    public string? TraceParent { get; set; }
}
