// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class AppMessageTaskHistory : Entity<Guid>
{
    public string TaskHistoryNo { get; protected set; } = string.Empty;

    public AppMessageTask MessageTask { get; protected set; } = default!;
}
