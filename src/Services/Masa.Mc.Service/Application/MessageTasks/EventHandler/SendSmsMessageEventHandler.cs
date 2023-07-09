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
    private readonly ILogger<SendSmsMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly II18n<DefaultResource> _i18n;

    public SendSmsMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsSender smsSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , ILogger<SendSmsMessageEventHandler> logger
        , IMessageTemplateRepository templateRepository
        , II18n<DefaultResource> i18n)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _logger = logger;
        _templateRepository = templateRepository;
        _i18n = i18n;
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

            var messageTemplate = await _templateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
            var insertMessageRecords = new List<MessageRecord>();

            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                messageRecord.SetDataValue(nameof(MessageTemplate.Sign), taskHistory.MessageTask.Sign);
                messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));

                var variables = messageRecord.Variables;
                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    messageRecord.SetDisplayName(messageTemplate.DisplayName);
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                        insertMessageRecords.Add(messageRecord);
                        continue;
                    }

                    variables = _messageTemplateDomainService.ConvertVariables(messageTemplate, messageRecord.Variables);
                }

                var smsMessage = new SmsMessage(item.ChannelUserIdentity, JsonSerializer.Serialize(variables));
                smsMessage.Properties.Add("SignName", taskHistory.MessageTask.Sign);
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
                    _logger.LogError(ex, "SendSmsMessageEventHandler");
                    messageRecord.SetResult(false, ex.Message);
                }

                insertMessageRecords.Add(messageRecord);
            }

            await _messageRecordRepository.AddRangeAsync(insertMessageRecords);

            taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        }
    }
}
