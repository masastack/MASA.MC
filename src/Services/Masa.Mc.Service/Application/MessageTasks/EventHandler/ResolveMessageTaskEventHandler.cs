// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

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
    public async Task QueryMessageTask(ResolveMessageTaskEvent eto)
    {
        var messageTask = (await _messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == eto.MessageTaskId);

        if (messageTask == null || messageTask.ReceiverType == ReceiverTypes.Broadcast)
        {
            eto.IsStop = true;
            return;
        }

        eto.MessageTask = messageTask;
    }

    [EventHandler(2)]
    public async Task FillReceiverUsers(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;

        var receiverUsers = messageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User)
            .Select(async x =>
            {
                var channelUserIdentity = await GetChannelUserIdentity(messageTask.Channel.Type, x.Receiver);
                var variables = x.Variables.Count == 0 ? messageTask.Variables : x.Variables;
                return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, variables);
            })
            .ToList();

        var result = await Task.WhenAll(receiverUsers);

        messageTask.ReceiverUsers.AddRange(result);

    }

    [EventHandler(3)]
    public async Task FillDepartmentUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;

        var orgIds = messageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization).Select(x => x.Receiver.SubjectId).Distinct();
        var receiverUsers = await GetMessageReceiverUser(messageTask.Channel.Type, ReceiverGroupItemTypes.Organization, orgIds.ToList(), messageTask.Variables);
        messageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(4)]
    public async Task FillRoleUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;

        var roleIds = messageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role).Select(x => x.Receiver.SubjectId).Distinct();
        var receiverUsers = await GetMessageReceiverUser(messageTask.Channel.Type, ReceiverGroupItemTypes.Role, roleIds.ToList(), messageTask.Variables);
        messageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(5)]
    public async Task FillTeamUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;

        var teamIds = messageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team).Select(x => x.Receiver.SubjectId).Distinct();
        var receiverUsers = await GetMessageReceiverUser(messageTask.Channel.Type, ReceiverGroupItemTypes.Team, teamIds.ToList(), messageTask.Variables);
        messageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(6)]
    public async Task FillReceiverGroupUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;

        var receiverGroupIds = messageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group).Select(x => x.Receiver.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var typeGroups = receiverGroup.Items.GroupBy(x => x.Type).ToList();
            foreach (var items in typeGroups)
            {
                if (items.Key == ReceiverGroupItemTypes.User)
                {
                    var userList = items.Select(x =>
                    {
                        var channelUserIdentity = x.Receiver.GetChannelUserIdentity(messageTask.Channel.Type);
                        return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, messageTask.Variables);
                    });
                    messageTask.ReceiverUsers.AddRange(userList);
                }
                else
                {
                    var subjectIds = items.Select(x => x.Receiver.SubjectId).Distinct().ToList();
                    var subjectUsers = await GetMessageReceiverUser(messageTask.Channel.Type, items.Key, subjectIds, messageTask.Variables);
                    messageTask.ReceiverUsers.AddRange(subjectUsers);
                }
            }
        }
    }

    [EventHandler(7)]
    public async Task CreateMessageTaskHistoryAsync(ResolveMessageTaskEvent eto)
    {
        var sendTime = DateTimeOffset.Now;
        var messageTask = eto.MessageTask;
        if (messageTask.SendRules.IsCustom)
        {
            var historyNum = messageTask.GetHistoryCount();
            var sendingCount = messageTask.GetSendingCount();

            var cronExpression = new CronExpression(messageTask.SendRules.CronExpression);
            cronExpression.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            for (int i = 0; i < historyNum; i++)
            {
                var nextExcuteTime = cronExpression.GetNextValidTimeAfter(sendTime);
                if (nextExcuteTime.HasValue)
                {
                    sendTime = nextExcuteTime.Value;
                    var taskHistoryNo = messageTask.GenerateHistoryNo();
                    var receiverUsers = messageTask.ReceiverUsers.Skip(i * sendingCount).Take(sendingCount).ToList();
                    var history = new MessageTaskHistory(messageTask.Id, taskHistoryNo, receiverUsers, false, sendTime);
                    await _messageTaskHistoryRepository.AddAsync(history);
                }
            }
        }
        else
        {
            var taskHistoryNo = messageTask.GenerateHistoryNo();
            var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, eto.MessageTask.ReceiverUsers, false, sendTime);
            history.ExecuteTask();
            await _messageTaskHistoryRepository.AddAsync(history);
        }

        if (!eto.MessageTask.SendRules.IsCustom)
        {
            eto.IsStop = true;
        }
    }

    [EventHandler(8)]
    public async Task AddSchedulerJobAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

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
            await _schedulerClient.SchedulerJobService.StartAsync(new SchedulerJobRequestBase { JobId = jobId, OperatorId = userId });
        }
    }

    private async Task<List<MessageReceiverUser>> GetMessageReceiverUser(ChannelTypes channelType, ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var authUsers = await GetAuthUsers(type, subjectIds);
        return authUsers.Select(x =>
        {
            var receiver = new Receiver(x.UserId, x.DisplayName, x.Avatar, x.PhoneNumber, x.Email);
            var channelUserIdentity = receiver.GetChannelUserIdentity(channelType);
            return new MessageReceiverUser(x.UserId, channelUserIdentity, variables);
        }).ToList();
    }

    private async Task<List<StaffModel>> GetAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds)
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
        return userList;
    }

    private async Task<string> GetChannelUserIdentity(ChannelTypes channelType, Receiver receiver)
    {
        var channelUserIdentity = receiver.GetChannelUserIdentity(channelType);
        if (string.IsNullOrEmpty(channelUserIdentity))
        {
            var authUser = await _authClient.UserService.FindByIdAsync(receiver.SubjectId);
            if (authUser != null)
            {
                receiver = new Receiver(authUser.Id, authUser.DisplayName, authUser.Avatar, authUser.PhoneNumber ?? string.Empty, authUser.Email ?? string.Empty);
                channelUserIdentity = receiver.GetChannelUserIdentity(channelType);
            }
        }
        return channelUserIdentity;
    }
}
