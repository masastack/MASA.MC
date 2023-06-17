// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

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
    private readonly ILogger<SendEmailMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public SendEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal
        , IEmailSender emailSender
        , ITemplateRenderer templateRenderer
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , ILogger<SendEmailMessageEventHandler> logger
        , IMessageTemplateRepository repository
        , II18n<DefaultResource> i18n)
    {
        _emailAsyncLocal = emailAsyncLocal;
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _logger = logger;
        _repository = repository;
        _i18n = i18n;
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

            var messageTemplate = await _repository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);

            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                eto.MessageData.RenderContent(item.Variables);

                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                try
                {
                    await _emailSender.SendAsync(
                        item.ChannelUserIdentity,
                        eto.MessageData.MessageContent.Title,
                        eto.MessageData.MessageContent.Content
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
}
