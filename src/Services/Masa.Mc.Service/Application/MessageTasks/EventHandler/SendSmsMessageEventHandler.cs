// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendSmsMessageEventHandler
{
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly ILogger<SendSmsMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ITemplateRenderer _templateRenderer;

    public SendSmsMessageEventHandler(SmsSenderFactory smsSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , ILogger<SendSmsMessageEventHandler> logger
        , IMessageTemplateRepository templateRepository
        , II18n<DefaultResource> i18n
        , ITemplateRenderer templateRenderer)
    {
        _smsSenderFactory = smsSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _logger = logger;
        _templateRepository = templateRepository;
        _i18n = i18n;
        _templateRenderer = templateRenderer;
    }

    [EventHandler(1)]
    public async Task ResolveAsync(SendSmsMessageEvent eto)
    {
        var channelId = eto.ChannelId;
        var taskHistory = eto.MessageTaskHistory;

        var messageTemplate = await _templateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
        var messageRecords = new List<MessageRecord>();

        foreach (var item in taskHistory.ReceiverUsers)
        {
            var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.Sign), taskHistory.MessageTask.Sign);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));

            if (eto.MessageData.MessageType == MessageEntityTypes.Template)
            {
                messageRecord.SetDisplayName(messageTemplate.DisplayName);
            }

            messageRecords.Add(messageRecord);
        }

        eto.Sign = taskHistory.MessageTask.Sign;
        eto.MessageRecords = messageRecords;
        eto.MessageTemplate = messageTemplate;
    }

    [EventHandler(2)]
    public async Task CheckAsync(SendSmsMessageEvent eto)
    {
        if (eto.MessageData.MessageType == MessageEntityTypes.Template)
        {
            var channelUserIdentitys = eto.MessageRecords.Select(x => x.ChannelUserIdentity).Distinct().ToList();
            var checkChannelUserIdentitys = await _messageTemplateDomainService.CheckSendUpperLimitAsync(eto.MessageTemplate, channelUserIdentitys);

            foreach (var messageRecord in eto.MessageRecords)
            {
                if (checkChannelUserIdentitys.Contains(messageRecord.ChannelUserIdentity))
                {
                    messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                    continue;
                }

                var variables = _messageTemplateDomainService.ConvertVariables(eto.MessageTemplate, messageRecord.Variables);
                eto.AddPhoneNumberVariable(messageRecord.ChannelUserIdentity, variables);
            }
        }
    }

    [EventHandler(3)]
    public async Task SendAsync(SendSmsMessageEvent eto)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == eto.ChannelId);
        var provider = (SmsProviders)channel.Provider;
        var options = _smsSenderFactory.GetOptions(provider, channel.ExtraProperties);
        var smsAsyncLoca = _smsSenderFactory.GetProviderAsyncLocal(provider);
        var smsSender = _smsSenderFactory.GetSender(provider);

        using (smsAsyncLoca.Change(options))
        {
            foreach (var item in eto.PhoneNumberVariables)
            {
                var phoneNumbers = item.Select(x => x.Key).ToList();
                var text = BuildSmsMessageText(provider, item, eto.MessageTemplate);
                var batchSmsMessage = new BatchSmsMessage(phoneNumbers, text);
                batchSmsMessage.Properties.Add("SignName", eto.Sign);
                batchSmsMessage.Properties.Add("TemplateCode", eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));

                try
                {
                    var response = await smsSender.SendBatchAsync(batchSmsMessage);
                    if (response.Success)
                    {
                        SetMessageRecordResult(eto, item, true, response.Message, response.MsgId);
                    }
                    else
                    {
                        SetMessageRecordResult(eto, item, false, response.Message, response.MsgId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendSmsMessageEventHandler");
                    SetMessageRecordResult(eto, item, false, ex.Message, string.Empty);
                }
            }
        }
    }

    [EventHandler(4)]
    public async Task SaveAsync(SendSmsMessageEvent eto)
    {
        await _messageRecordRepository.AddRangeAsync(eto.MessageRecords);

        var messageTaskHistory = eto.MessageTaskHistory;

        var result = !eto.MessageRecords.Any(x => x.Success != true) ? MessageTaskHistoryStatuses.Success : (!eto.MessageRecords.Any(x => x.Success == true) ? MessageTaskHistoryStatuses.Fail : MessageTaskHistoryStatuses.PartialFailure);
        messageTaskHistory.SetResult(result);
        await _messageTaskHistoryRepository.UpdateAsync(messageTaskHistory);
    }

    private void SetMessageRecordResult(SendSmsMessageEvent eto, Dictionary<string, ExtraPropertyDictionary> phoneNumberVariable, bool success, string message, string msgId)
    {
        foreach (var item in phoneNumberVariable)
        {
            var record = eto.MessageRecords.FirstOrDefault(x => !x.Success.HasValue && x.ChannelUserIdentity == item.Key);
            if (record == null)
                continue;

            record.SetResult(success, message, DateTimeOffset.UtcNow, msgId);
        }
    }

    private string BuildSmsMessageText(SmsProviders provider, Dictionary<string, ExtraPropertyDictionary> phoneNumberVariable, MessageTemplate messageTemplate)
    {
        if (provider == SmsProviders.Aliyun)
        {
            return JsonSerializer.Serialize(phoneNumberVariable.Select(x => x.Value));
        }
        else if (provider == SmsProviders.YunMas)
        {
            var dic = new Dictionary<string, string>();
            foreach (var item in phoneNumberVariable)
            {
                var content = _templateRenderer.Render(messageTemplate.MessageContent.Content, item.Value);
                dic.TryAdd(item.Key, content);
            }

            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(dic, settings);
        }
        else
        {
            return string.Empty;
        }
    }
}
