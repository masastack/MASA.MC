﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Jobs;

public class MessageTaskExecuteJobConsts
{
    public const string JOB_APP_IDENTITY = "masa-mc-job";
    public const string JOB_ENTRY_ASSEMBLY = "Masa.Mc.Service.Admin.dll";
    public const string JOB_ENTRY_METHOD = "Masa.Mc.Service.Admin.Jobs.MessageTaskExecuteJob";
    public static ActivitySource ActivitySource { get; private set; } = new("Masa.Mc.Background");
}
