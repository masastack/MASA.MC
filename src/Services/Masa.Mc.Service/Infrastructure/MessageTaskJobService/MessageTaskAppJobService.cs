// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.MessageTaskJobService;

public class MessageTaskAppJobService : IMessageTaskJobService
{
    private readonly ISchedulerClient _schedulerClient;
    private readonly IMasaStackConfig _masaStackConfig;

    public MessageTaskAppJobService(ISchedulerClient schedulerClient, IMasaStackConfig masaStackConfig)
    {
        _schedulerClient = schedulerClient;
        _masaStackConfig = masaStackConfig;
    }

    public async Task<bool> DisableJobAsync(Guid jobId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerJobService.EnableAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = operatorId });
    }

    public async Task<bool> EnableJobAsync(Guid jobId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerJobService.DisableAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = operatorId });
    }

    public async Task<Guid> RegisterJobAsync(Guid messageTaskId, string cronExpression, Guid operatorId, string jobName)
    {
        var mcUrl = _masaStackConfig.GetMcServiceDomain();
        var request = new UpsertSchedulerJobRequest
        {
            ProjectIdentity = MasaStackConsts.MC_SYSTEM_ID,
            Name = jobName,
            JobType = JobTypes.JobApp,
            CronExpression = cronExpression,
            OperatorId = operatorId,
            JobAppConfig = new SchedulerJobAppConfig
            {
                JobAppIdentity = MessageTaskExecuteJobConsts.JOB_APP_IDENTITY,
                JobEntryAssembly = MessageTaskExecuteJobConsts.JOB_ENTRY_ASSEMBLY,
                JobEntryClassName = MessageTaskExecuteJobConsts.JOB_ENTRY_METHOD,
                JobParams = messageTaskId.ToString(),
            },
            NotifyUrl = $"{mcUrl}/api/message-task/HandleJobStatusNotify"
        };

        var jobId = await _schedulerClient.SchedulerJobService.AddAsync(request);

        return jobId;
    }

    public async Task<bool> StartJobAsync(Guid jobId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerJobService.StartAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = operatorId });
    }

    public async Task<bool> StopTaskAsync(Guid taskId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerTaskService.StopAsync(new SchedulerTaskRequestBase { TaskId = taskId, OperatorId = operatorId });
    }
}
