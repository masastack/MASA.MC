﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

[Obsolete("To be abandoned")]
public record SendOrdinaryMessageTaskCommand(SendOrdinaryMessageTaskInputDto inputDto) : Command
{
}
