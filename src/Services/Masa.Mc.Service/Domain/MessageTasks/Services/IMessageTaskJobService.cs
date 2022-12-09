// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public interface IMessageTaskJobService
{
    Task<Guid> RegisterJobAsync(Guid messageTaskId, string cronExpression, Guid operatorId, string jobName);

    Task<bool> StartJobAsync(Guid jobId, Guid operatorId);

    Task<bool> EnableJobAsync(Guid jobId, Guid operatorId);

    Task<bool> DisableJobAsync(Guid jobId, Guid operatorId);

    Task<bool> StopTaskAsync(Guid taskId, Guid operatorId);
}
