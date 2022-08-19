// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IReceiverGroupRepository _receiverGroupRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IAuthClient _authClient;
    private readonly ISchedulerClient _schedulerClient;
    private readonly IUserContext _userContext;

    public ResolveMessageTaskEventHandler(IChannelRepository channelRepository
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IReceiverGroupRepository receiverGroupRepository
        , IDomainEventBus eventBus
        , IAuthClient authClient
        , ISchedulerClient schedulerClient
        , IUserContext userContext)
    {
        _channelRepository = channelRepository;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _receiverGroupRepository = receiverGroupRepository;
        _eventBus = eventBus;
        _authClient = authClient;
        _schedulerClient = schedulerClient;
        _userContext = userContext;
    }

    [EventHandler(1)]
    public async void FillReceiverUsers(ResolveMessageTaskEvent eto)
    {
        eto.MessageTask = (await _messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == eto.MessageTaskId);
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverUsers = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User)
            .Select(x => new MessageReceiverUser
            {
                UserId = x.SubjectId,
                DisplayName = x.DisplayName,
                Account = x.Account,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Variables = x.Variables.Count == 0 ? eto.MessageTask.Variables : x.Variables,
            })
            .ToList();
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(2)]
    public async Task FillDepartmentUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var orgIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Organization, orgIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(3)]
    public async Task FillRoleUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var roleIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Role, roleIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(4)]
    public async Task FillTeamUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var teamIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Team, teamIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(5)]
    public async Task FillReceiverGroupUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverGroupIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group).Select(x => x.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var typeGroups = receiverGroup.Items.GroupBy(x => x.Type).ToList();
            foreach (var items in typeGroups)
            {
                if (items.Key == ReceiverGroupItemTypes.User)
                {
                    var userList = items.Select(x => new MessageReceiverUser
                    {
                        UserId = x.SubjectId,
                        DisplayName = x.DisplayName,
                        PhoneNumber = x.PhoneNumber,
                        Email = x.Email,
                        Variables = eto.MessageTask.Variables,
                    });
                    eto.MessageTask.ReceiverUsers.AddRange(userList);
                }
                else
                {
                    var subjectIds = items.Select(x => x.SubjectId).Distinct().ToList();
                    var subjectUsers = await ResolveAuthUsers(items.Key, subjectIds, eto.MessageTask.Variables);
                    eto.MessageTask.ReceiverUsers.AddRange(subjectUsers);
                }
            }
        }
    }

    [EventHandler(6)]
    public async Task CreateMessageTaskHistoryAsync(ResolveMessageTaskEvent eto)
    {
        var sendTime = DateTimeOffset.Now;
        if (eto.MessageTask.SendRules.IsCustom)
        {
            var totalCount = eto.MessageTask.ReceiverUsers.Count;
            var sendingCount = (int)eto.MessageTask.SendRules.SendingCount;
            if (sendingCount == 0)
            {
                sendingCount = totalCount;
            }
            var historyNum = (long)Math.Ceiling((double)totalCount / sendingCount);

            if (eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
            {
                historyNum = 1;
            }

            var cronExpression = new CronExpression(eto.MessageTask.SendRules.CronExpression);
            cronExpression.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            for (int i = 0; i < historyNum; i++)
            {
                var nextExcuteTime = cronExpression.GetNextValidTimeAfter(sendTime);
                if (nextExcuteTime.HasValue)
                {
                    sendTime = nextExcuteTime.Value;
                    var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
                    var receiverUsers = eto.MessageTask.ReceiverUsers.Skip(i * sendingCount).Take(sendingCount).ToList();
                    var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, receiverUsers, false, sendTime);
                    await _messageTaskHistoryRepository.AddAsync(history);
                }
            }
        }
        else
        {
            var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
            var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, eto.MessageTask.ReceiverUsers, false, sendTime);
            history.ExecuteTask();
            await _messageTaskHistoryRepository.AddAsync(history);
        }
    }

    [EventHandler(7)]
    public async Task AddSchedulerJobAsync(ResolveMessageTaskEvent eto)
    {
        if (!eto.MessageTask.SendRules.IsCustom)
        {
            return;
        }

        var cronExpression = eto.MessageTask.SendRules.CronExpression;
        var userId = _userContext.GetUserId<Guid>();
        var request = new AddSchedulerJobRequest
        {
            ProjectIdentity = MasaStackConsts.MC_SYSTEM_ID,
            Name = eto.MessageTask.DisplayName,
            JobType = JobTypes.JobApp,
            CronExpression = cronExpression,
            OperatorId = userId == default ? eto.OperatorId : userId,
            JobAppConfig = new SchedulerJobAppConfig
            {
                JobAppIdentity = MessageTaskExecuteJobConsts.JOB_APP_IDENTITY,
                JobEntryAssembly = MessageTaskExecuteJobConsts.JOB_ENTRY_ASSEMBLY,
                JobEntryClassName = MessageTaskExecuteJobConsts.JOB_ENTRY_METHOD,
                JobParams = eto.MessageTaskId.ToString(),
            }
        };

        var jobId = await _schedulerClient.SchedulerJobService.AddAsync(request);
        eto.MessageTask.SetJobId(jobId);
        await _messageTaskRepository.UpdateAsync(eto.MessageTask);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();

        if (string.IsNullOrEmpty(cronExpression) && jobId != default)
        {
            await _schedulerClient.SchedulerJobService.StartAsync(new BaseSchedulerJobRequest { JobId = jobId, OperatorId = userId });
        }
    }

    private MessageReceiverUser MapToMessageReceiverUser(StaffModel staff, ExtraPropertyDictionary variables)
    {
        return new MessageReceiverUser
        {
            UserId = staff.Id,
            DisplayName = staff.Name ?? staff.DisplayName ?? string.Empty,
            Account = staff.Account,
            PhoneNumber = staff.PhoneNumber ?? string.Empty,
            Email = staff.Email ?? string.Empty,
            Variables = variables,
        };
    }

    private async Task<List<MessageReceiverUser>> ResolveAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var userList = new List<StaffModel>();
        switch (type)
        {
            case ReceiverGroupItemTypes.User:
                break;
            case ReceiverGroupItemTypes.Organization:
                foreach (var orgId in subjectIds)
                {
                    var departmentUsers = await _authClient.UserService.GetListByDepartmentAsync(orgId);
                    userList.AddRange(departmentUsers);
                }
                break;
            case ReceiverGroupItemTypes.Role:
                foreach (var roleId in subjectIds)
                {
                    var roleUsers = await _authClient.UserService.GetListByRoleAsync(roleId);
                    userList.AddRange(roleUsers);
                }
                break;
            case ReceiverGroupItemTypes.Team:
                foreach (var teamId in subjectIds)
                {
                    var teamUsers = await _authClient.UserService.GetListByTeamAsync(teamId);
                    userList.AddRange(teamUsers);
                }
                break;
            default:
                break;
        }
        return userList.Select(x => MapToMessageReceiverUser(x, variables))
            .ToList();
    }
}
