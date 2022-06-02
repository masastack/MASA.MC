﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskReceiverInputDto
{
    public string Filter { get; set; } = string.Empty;

    public GetMessageTaskReceiverInputDto(string filter)
    {
        Filter = filter;
    }
}
