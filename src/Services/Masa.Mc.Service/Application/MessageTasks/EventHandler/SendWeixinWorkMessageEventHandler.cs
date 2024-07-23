// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWeixinWorkMessageEventHandler
{
    private readonly IWeixinWorkMessageAsyncLocal _weixinWorkMessageAsyncLocal;
    private readonly IWeixinWorkSender _weixinWorkMessageSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly II18n<DefaultResource> _i18n;

    public SendWeixinWorkMessageEventHandler(IWeixinWorkMessageAsyncLocal weixinWorkMessageAsyncLocal
        , IWeixinWorkSender weixinWorkMessageSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageTemplateRepository templateRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , II18n<DefaultResource> i18n)
    {
        _weixinWorkMessageAsyncLocal = weixinWorkMessageAsyncLocal;
        _weixinWorkMessageSender = weixinWorkMessageSender;
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
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new WeixinWorkOptions
        {
            CorpId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpId)) ?? string.Empty,
            CorpSecret = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.CorpSecret)) ?? string.Empty,
            AgentId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkOptions.AgentId)) ?? string.Empty,
        };

        var taskHistory = eto.MessageTaskHistory;
        var messageRecords = new List<MessageRecord>();

        var checkChannelUserIdentitys = new List<string>();
        if (eto.MessageData.MessageType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _templateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
            var channelUserIdentitys = eto.MessageTaskHistory.ReceiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToList();
            checkChannelUserIdentitys = await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, channelUserIdentitys);
        }

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

        var toUser = GetToUser(taskHistory.MessageTask.ReceiverType, messageRecords.Select(x => x.ChannelUserIdentity).ToList());

        using (_weixinWorkMessageAsyncLocal.Change(options))
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

        taskHistory.SetResult(!messageRecords.Any(x => x.Success != true) ? MessageTaskHistoryStatuses.Success : (messageRecords.Any(x => x.Success == true) ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async Task<WeixinWorkMessageResponseBase> SendAsync(MessageData messageData, string toUser)
    {
        var type = messageData.GetDataValue<int>(BusinessConsts.MESSAGE_TYPE);
        if (type == (int)WeixinWorkTemplateTypes.TextCard)
        {
            var message = new WeixinWorkTextCardMessage(toUser, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.JumpUrl);
            return await _weixinWorkMessageSender.SendTextCardAsync(message);
        }
        else
        {
            var message = new WeixinWorkTextMessage(toUser, messageData.MessageContent.Content);
            return await _weixinWorkMessageSender.SendTextAsync(message);
        }
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
            var messageRecord = messageRecords.FirstOrDefault(x => x.ChannelUserIdentity == item);
            messageRecord?.SetResult(false, "invaliduser");
        }
    }

    private void SetResult(List<MessageRecord> messageRecords, bool success, string failureReason)
    {
        foreach (var item in messageRecords.Where(x => x.Success == null))
        {
            item.SetResult(success, failureReason);
        }
    }
}
