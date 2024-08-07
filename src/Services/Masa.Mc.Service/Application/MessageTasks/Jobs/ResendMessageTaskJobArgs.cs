﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResendMessageTaskJobArgs : MultiEnvironment
{
    public Guid MessageTaskId { get; set; }

    public string? TraceParent { get; set; }
}
