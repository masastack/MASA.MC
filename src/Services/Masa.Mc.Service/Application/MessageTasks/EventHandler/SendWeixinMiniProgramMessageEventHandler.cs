// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWeixinMiniProgramMessageEventHandler
{
    private const string OnlySupportsTemplateMessage = "Weixin mini program channel only supports subscribe template messages";
    private readonly IProviderAsyncLocal<IWeixinMiniProgramOptions> _asyncLocal;
    private readonly IWeixinMiniProgramSender _sender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ILogger<SendWeixinMiniProgramMessageEventHandler> _logger;

    public SendWeixinMiniProgramMessageEventHandler(IProviderAsyncLocal<IWeixinMiniProgramOptions> asyncLocal
        , IWeixinMiniProgramSender sender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageTemplateRepository templateRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , II18n<DefaultResource> i18n
        , ILogger<SendWeixinMiniProgramMessageEventHandler> logger)
    {
        _asyncLocal = asyncLocal;
        _sender = sender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _templateRepository = templateRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _i18n = i18n;
        _logger = logger;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendWeixinMiniProgramMessageEvent eto)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == eto.ChannelId);
        var taskHistory = eto.MessageTaskHistory;
        if (channel is null)
        {
            taskHistory.SetResult(MessageTaskHistoryStatuses.Fail);
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
            return;
        }

        var messageTemplate = eto.MessageData.MessageType == MessageEntityTypes.Template
            ? await _templateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false)
            : null;
        var templateId = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
        var options = channel.GetWeixinMiniProgramOptions();
        var messageRecords = new List<MessageRecord>();

        using (_asyncLocal.Change(options))
        {
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var renderedData = eto.MessageData.RenderForReceiver(item.Variables);
                var displayName = string.IsNullOrEmpty(renderedData.MessageContent.Title) ? taskHistory.MessageTask.DisplayName : renderedData.MessageContent.Title;
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, displayName, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), templateId);

                await SendOrBlockAsync(messageRecord, messageTemplate, renderedData, templateId);
                messageRecords.Add(messageRecord);
            }
        }

        await _messageRecordRepository.AddRangeAsync(messageRecords);
        await UpdateTaskHistoryAsync(taskHistory, messageRecords);
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSimpleWeixinMiniProgramMessageEvent eto)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == eto.ChannelCode);
        if (channel is null)
        {
            return;
        }

        var templateId = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
        var messageEntityId = eto.MessageData.GetDataValue<Guid>(nameof(MessageTemplate.Id));
        var messageTemplate = eto.MessageData.MessageType == MessageEntityTypes.Template
            ? await _templateRepository.FindAsync(x => x.Id == messageEntityId, false)
            : null;
        var messageRecord = new MessageRecord(Guid.Empty, eto.ChannelUserIdentity, channel.Id, Guid.Empty, Guid.Empty, eto.Variables, eto.MessageData.MessageContent.Title, DateTimeOffset.UtcNow, eto.SystemId);
        messageRecord.SetMessageEntity(eto.MessageData.MessageType, messageEntityId);
        messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), templateId);
        messageRecord.SetDisplayName(eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));

        using (_asyncLocal.Change(channel.GetWeixinMiniProgramOptions()))
        {
            await SendOrBlockAsync(messageRecord, messageTemplate, eto.MessageData, templateId);
        }

        await _messageRecordRepository.AddAsync(messageRecord);
    }

    private async Task SendOrBlockAsync(MessageRecord messageRecord, MessageTemplate? messageTemplate, MessageData messageData, string templateId)
    {
        if (messageData.MessageType != MessageEntityTypes.Template)
        {
            messageRecord.SetResult(false, OnlySupportsTemplateMessage);
            return;
        }

        if (messageTemplate is null)
        {
            messageRecord.SetResult(false, _i18n.T("MessageTemplate"));
            return;
        }

        if (string.IsNullOrWhiteSpace(templateId))
        {
            messageRecord.SetResult(false, "TemplateId is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(messageRecord.ChannelUserIdentity))
        {
            messageRecord.SetResult(false, "OpenId is required");
            return;
        }

        if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
        {
            messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
            return;
        }

        var message = new WeixinMiniProgramSubscribeMessage(
            messageRecord.ChannelUserIdentity,
            templateId,
            messageData.MessageContent.GetJumpUrl(),
            BuildTemplateData(messageTemplate, messageRecord.Variables));

        try
        {
            var response = await _sender.SendSubscribeMessageAsync(message);
            if (response.ErrCode == 0)
            {
                messageRecord.SetResult(true, string.Empty, DateTimeOffset.UtcNow, response.MsgId);
            }
            else
            {
                messageRecord.SetResult(false, response.ErrMsg, DateTimeOffset.UtcNow, response.MsgId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendWeixinMiniProgramMessageEventHandler");
            messageRecord.SetResult(false, ex.Message);
        }
    }

    private Dictionary<string, string> BuildTemplateData(MessageTemplate messageTemplate, ExtraPropertyDictionary variables)
    {
        var convertedVariables = _messageTemplateDomainService.ConvertVariables(messageTemplate, variables);
        return convertedVariables.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? string.Empty);
    }

    private async Task UpdateTaskHistoryAsync(MessageTaskHistory taskHistory, List<MessageRecord> messageRecords)
    {
        taskHistory.SetResult(!messageRecords.Any(x => x.Success != true) ? MessageTaskHistoryStatuses.Success : (messageRecords.Any(x => x.Success == true) ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }
}
