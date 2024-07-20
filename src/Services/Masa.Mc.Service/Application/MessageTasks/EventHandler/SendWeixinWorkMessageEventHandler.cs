// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWeixinWorkMessageEventHandler
{
    private readonly IWeixinWorkMessageAsyncLocal _weixinWorkMessageAsyncLocal;
    private readonly IWeixinWorkMessageSender _weixinWorkMessageSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;

    public SendWeixinWorkMessageEventHandler(IWeixinWorkMessageAsyncLocal weixinWorkMessageAsyncLocal
        , IWeixinWorkMessageSender weixinWorkMessageSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository)
    {
        _weixinWorkMessageAsyncLocal = weixinWorkMessageAsyncLocal;
        _weixinWorkMessageSender = weixinWorkMessageSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendWeixinWorkMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new WeixinWorkMessageOptions
        {
            CorpId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkMessageOptions.CorpId)) ?? string.Empty,
            CorpSecret = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkMessageOptions.CorpSecret)) ?? string.Empty,
            AgentId = channel?.ExtraProperties.GetProperty<string>(nameof(WeixinWorkMessageOptions.AgentId)) ?? string.Empty,
        };

        var taskHistory = eto.MessageTaskHistory;

        var messageRecords = new List<MessageRecord>();
        foreach (var item in eto.MessageTaskHistory.ReceiverUsers)
        {
            var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, eto.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));

            messageRecords.Add(messageRecord);
        }

        using (_weixinWorkMessageAsyncLocal.Change(options))
        {
            var toUser = GetToUser(eto);
            var message = new WeixinWorkTextCardMessage(toUser, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.MessageContent.JumpUrl);
            var response = await _weixinWorkMessageSender.SendTextCardAsync(message);

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

        taskHistory.SetResult(!messageRecords.Any(x=>x.Success != true) ? MessageTaskHistoryStatuses.Success : (messageRecords.Any(x => x.Success == true) ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private string GetToUser(SendWeixinWorkMessageEvent eto)
    {
        if (eto.MessageTaskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
        {
            return "@all";
        }

        var channelUserIdentitys = eto.MessageTaskHistory.ReceiverUsers.Select(x => x.ChannelUserIdentity);
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
