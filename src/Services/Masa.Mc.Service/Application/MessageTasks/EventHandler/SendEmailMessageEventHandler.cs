// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendEmailMessageEventHandler
{
    private readonly IEmailAsyncLocal _emailAsyncLocal;
    private readonly IEmailSender _emailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly MessageRecordDomainService _messageRecordDomainService;
    private readonly ILogger<SendEmailMessageEventHandler> _logger;

    public SendEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal
        , IEmailSender emailSender
        , ITemplateRenderer templateRenderer
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , MessageRecordDomainService messageRecordDomainService
        , ILogger<SendEmailMessageEventHandler> logger)
    {
        _emailAsyncLocal = emailAsyncLocal;
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _messageRecordDomainService = messageRecordDomainService;
        _logger = logger;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendEmailMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new SmtpEmailOptions
        {
            Host = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Smtp)),
            Port = channel.ExtraProperties.GetProperty<int>(nameof(EmailChannelOptions.Port)),
            UserName = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName)),
            Password = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Password)),
            EnableSsl = channel.ExtraProperties.GetProperty<bool>(nameof(EmailChannelOptions.Ssl)),
            DefaultFromAddress = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName))
        };
        using (_emailAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            int okCount = 0;
            int totalCount = taskHistory.ReceiverUsers.Count;
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)), taskHistory.SendTime);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                _messageRecordDomainService.SetUserInfo(messageRecord, item);
                TemplateRenderer(eto.MessageData, item.Variables);
                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    var perDayLimit = eto.MessageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageRecord.MessageEntityId, perDayLimit, item.UserId))
                    {
                        messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                try
                {
                    await _emailSender.SendAsync(
                        item.Email,
                        eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)),
                        eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Content))
                    );
                    messageRecord.SetResult(true, string.Empty);
                    okCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendEmailMessageEventHandler");
                    messageRecord.SetResult(false, ex.Message);
                }

                await _messageRecordRepository.AddAsync(messageRecord);
            }
            taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        }
    }

    private async void TemplateRenderer(MessageData messageData, ExtraPropertyDictionary Variables)
    {
        messageData.SetDataValue(nameof(MessageTemplate.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), Variables));
        messageData.SetDataValue(nameof(MessageTemplate.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), Variables));
    }
}
