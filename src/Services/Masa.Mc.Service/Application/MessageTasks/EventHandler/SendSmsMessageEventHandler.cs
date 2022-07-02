// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendSmsMessageEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsSender _smsSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;

    public SendSmsMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsSender smsSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSmsMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            int okCount = 0;
            int totalCount = taskHistory.ReceiverUsers.Count;
            foreach (var item in taskHistory.ReceiverUsers)
            {

                var messageRecord = new MessageRecord(item.UserId, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));
                SetUserInfo(messageRecord, item);

                var perDayLimit = eto.MessageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(perDayLimit, item.UserId))
                {
                    messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                }
                else
                {
                    var smsMessage = new SmsMessage(item.PhoneNumber, JsonSerializer.Serialize(item.Variables));
                    smsMessage.Properties.Add("SignName", eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Sign)));
                    smsMessage.Properties.Add("TemplateCode", eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));
                    try
                    {
                        var response = await _smsSender.SendAsync(smsMessage) as SmsSendResponse;
                        if (response.Success)
                        {
                            messageRecord.SetResult(true, string.Empty);
                            okCount++;
                        }
                        else
                        {
                            messageRecord.SetResult(false, response.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageRecord.SetResult(false, ex.Message);
                    }
                }

                await _messageRecordRepository.AddAsync(messageRecord);
            }
            taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        }
    }

    private void SetUserInfo(MessageRecord messageRecord, MessageReceiverUser item)
    {
        messageRecord.SetDataValue(nameof(item.DisplayName), item.DisplayName);
        messageRecord.SetDataValue(nameof(item.Email), item.Email);
        messageRecord.SetDataValue(nameof(item.PhoneNumber), item.PhoneNumber);
    }
}
