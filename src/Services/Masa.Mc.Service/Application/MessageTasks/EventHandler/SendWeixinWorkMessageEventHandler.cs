// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWeixinWorkMessageEventHandler
{
    private readonly IWeixinWorkAsyncLocal _weixinWorkAsyncLocal;
    private readonly IWeixinWorkSender _weixinWorkSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly II18n<DefaultResource> _i18n;

    public SendWeixinWorkMessageEventHandler(IWeixinWorkAsyncLocal weixinWorkAsyncLocal
        , IWeixinWorkSender weixinWorkSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageTemplateRepository templateRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , II18n<DefaultResource> i18n)
    {
        _weixinWorkAsyncLocal = weixinWorkAsyncLocal;
        _weixinWorkSender = weixinWorkSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _templateRepository = templateRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendWeixinWorkMessageEvent eto)
    {
        var options = await GetOptionsAsync(eto.ChannelId);

        var taskHistory = eto.MessageTaskHistory;
        var messageRecords = new List<MessageRecord>();

        var checkChannelUserIdentitys = await GetCheckChannelUserIdentitysAsync(eto);

        foreach (var item in eto.MessageTaskHistory.ReceiverUsers)
        {
            var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, eto.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));

            if (checkChannelUserIdentitys.Contains(messageRecord.ChannelUserIdentity))
            {
                messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                continue;
            }

            messageRecords.Add(messageRecord);
        }

        var channelUserIdentitys = messageRecords.Where(x => !x.Success.HasValue).Select(x => x.ChannelUserIdentity).ToList();
        var toUser = GetToUser(taskHistory.MessageTask.ReceiverType, channelUserIdentitys);

        using (_weixinWorkAsyncLocal.Change(options))
        {
            var response = await SendAsync(eto.MessageData, toUser);

            if (response.ErrCode != 0)
            {
                SetResult(messageRecords, false, response.ErrMsg);
            }
            else
            {
                if (!response.InvalidUser.IsNullOrEmpty())
                {
                    SetInvalidResult(messageRecords, response.InvalidUser);
                }

                SetResult(messageRecords, true, string.Empty);
            }
        }

        await _messageRecordRepository.AddRangeAsync(messageRecords);

        await UpdateTaskHistoryAsync(taskHistory, messageRecords);
    }

    private async Task<WeixinWorkMessageResponseBase> SendAsync(MessageData messageData, string toUser)
    {
        var type = messageData.GetDataValue<int>(BusinessConsts.MESSAGE_TYPE);
        if (type == (int)WeixinWorkTemplateTypes.TextCard)
        {
            var message = new WeixinWorkTextCardMessage(toUser, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.JumpUrl);
            return await _weixinWorkSender.SendTextCardAsync(message);
        }
        else
        {
            var message = new WeixinWorkTextMessage(toUser, messageData.MessageContent.Content);
            return await _weixinWorkSender.SendTextAsync(message);
        }
    }

    private async Task<WeixinWorkOptions> GetOptionsAsync(Guid channelId)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == channelId);
        var options = new WeixinWorkOptions
        {
            CorpId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpId)) ?? string.Empty,
            CorpSecret = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpSecret)) ?? string.Empty,
            AgentId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.AgentId)) ?? string.Empty,
        };
        return options;
    }

    private async Task<List<string>> GetCheckChannelUserIdentitysAsync(SendWeixinWorkMessageEvent eto)
    {
        var checkChannelUserIdentitys = new List<string>();
        if (eto.MessageData.MessageType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _templateRepository.FindAsync(x => x.Id == eto.MessageTaskHistory.MessageTask.EntityId, false);
            checkChannelUserIdentitys = await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, eto.MessageTaskHistory.ReceiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToList());
        }
        return checkChannelUserIdentitys;
    }

    private async Task UpdateTaskHistoryAsync(MessageTaskHistory taskHistory, List<MessageRecord> messageRecords)
    {
        taskHistory.SetResult(!messageRecords.Any(x => x.Success != true) ? MessageTaskHistoryStatuses.Success : (messageRecords.Any(x => x.Success == true) ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private string GetToUser(ReceiverTypes receiverType, List<string> channelUserIdentitys)
    {
        if (receiverType == ReceiverTypes.Broadcast)
        {
            return "@all";
        }

        return string.Join("|", channelUserIdentitys);
    }

    private void SetInvalidResult(List<MessageRecord> messageRecords, string invalidUser)
    {
        var invalidUsers = invalidUser.Split('|').ToList();

        foreach (var item in invalidUsers)
        {
            var messageRecord = messageRecords.FirstOrDefault(x => !x.Success.HasValue && x.ChannelUserIdentity == item);
            messageRecord?.SetResult(false, "invaliduser");
        }
    }

    private void SetResult(List<MessageRecord> messageRecords, bool success, string failureReason)
    {
        foreach (var item in messageRecords.Where(x => !x.Success.HasValue))
        {
            item.SetResult(success, failureReason);
        }
    }
}
