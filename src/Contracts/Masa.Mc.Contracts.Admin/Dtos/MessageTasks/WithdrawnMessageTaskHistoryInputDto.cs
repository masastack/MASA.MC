// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class WithdrawnMessageTaskHistoryInputDto
{
    public Guid MessageTaskId { get; set; }
    public Guid HistoryId { get; set; }
}
