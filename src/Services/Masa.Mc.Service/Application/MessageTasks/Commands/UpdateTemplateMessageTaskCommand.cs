﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands;

public record UpdateTemplateMessageTaskCommand(Guid MessageTaskId, MessageTaskUpsertDto MessageTask) : Command
{
}
