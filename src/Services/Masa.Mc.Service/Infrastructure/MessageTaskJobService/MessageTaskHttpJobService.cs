// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.MessageTaskJobService;

public class MessageTaskHttpJobService : IMessageTaskJobService
{
    private readonly ISchedulerClient _schedulerClient;
    private readonly IMasaConfiguration _configuration;
    private readonly IMasaStackConfig _masaStackConfig;

    public MessageTaskHttpJobService(ISchedulerClient schedulerClient
        , IMasaConfiguration configuration
        , IMasaStackConfig masaStackConfig)
    {
        _schedulerClient = schedulerClient;
        _configuration = configuration;
        _masaStackConfig = masaStackConfig;
    }

    public async Task<bool> DisableJobAsync(Guid jobId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerJobService.DisableAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = operatorId });
    }

    public async Task<bool> EnableJobAsync(Guid jobId, Guid operatorId)
    {
        return await _schedulerClient.SchedulerJobService.EnableAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = operatorId });
    }

    public async Task<Guid> RegisterJobAsync(Guid messageTaskId, string cronExpression, Guid operatorId, string jobName)
    {
        var mcUrl = _masaStackConfig.GetMcServiceDomain();
        var parameters = new List<KeyValuePair<string, string>>() { new(nameof(messageTaskId), messageTaskId.ToString()) };

        var request = new AddSchedulerJobRequest
        {
            ProjectIdentity = MasaStackConsts.MC_SYSTEM_ID,
            Name = jobName,
            JobType = JobTypes.Http,
            CronExpression = cronExpression,
            OperatorId = operatorId,
            HttpConfig = new SchedulerJobHttpConfig
            {
                HttpMethod = BuildingBlocks.StackSdks.Scheduler.Enum.HttpMethods.POST,
                RequestUrl = $"{mcUrl}/api/message-task/Execute",
                HttpParameters = parameters
            }
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
