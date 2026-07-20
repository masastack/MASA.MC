// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendEmailMessageEventHandler
{
    private readonly IEmailAsyncLocal _emailAsyncLocal;
    private readonly IEmailSender _emailSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly ILogger<SendEmailMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public SendEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal
        , IEmailSender emailSender
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
        var taskHistory = eto.MessageTaskHistory;
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == eto.ChannelId);
        if (channel is null)
        {
            _logger.LogWarning("Channel not found for email message send. ChannelId: {ChannelId}", eto.ChannelId);
            taskHistory.SetResult(MessageTaskHistoryStatuses.Fail);
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
            return;
        }

        var options = channel.GetSmtpEmailOptions();
        using (_emailAsyncLocal.Change(options))
        {
            int okCount = 0;
            int totalCount = taskHistory.ReceiverUsers.Count;

            var messageTemplate = await _repository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
            var insertMessageRecords = new List<MessageRecord>();

            foreach (var item in taskHistory.ReceiverUsers)
            {
                var renderedData = eto.MessageData.RenderForReceiver(item.Variables);

                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, renderedData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);

                if (renderedData.MessageType == MessageEntityTypes.Template)
                {
                    if (messageTemplate is null)
                    {
                        messageRecord.SetResult(false, _i18n.T("MessageTemplate"));
                        insertMessageRecords.Add(messageRecord);
                        continue;
                    }

                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                        insertMessageRecords.Add(messageRecord);
                        continue;
                    }
                }

                try
                {
                    await _emailSender.SendAsync(
                        item.ChannelUserIdentity,
                        renderedData.MessageContent.Title,
                        renderedData.MessageContent.Content
                    );
                    messageRecord.SetResult(true, string.Empty);
                    okCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendEmailMessageEventHandler");
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
