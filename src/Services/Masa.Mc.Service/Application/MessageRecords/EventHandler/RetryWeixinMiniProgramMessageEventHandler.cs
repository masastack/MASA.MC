// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryWeixinMiniProgramMessageEventHandler
{
    private readonly IProviderAsyncLocal<IWeixinMiniProgramOptions> _asyncLocal;
    private readonly IWeixinMiniProgramSender _sender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ILogger<RetryWeixinMiniProgramMessageEventHandler> _logger;

    public RetryWeixinMiniProgramMessageEventHandler(IProviderAsyncLocal<IWeixinMiniProgramOptions> asyncLocal
        , IWeixinMiniProgramSender sender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTemplateRepository templateRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , II18n<DefaultResource> i18n
        , ILogger<RetryWeixinMiniProgramMessageEventHandler> logger)
    {
        _asyncLocal = asyncLocal;
        _sender = sender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _templateRepository = templateRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _i18n = i18n;
        _logger = logger;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetryWeixinMiniProgramMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null) return;

        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == messageRecord.ChannelId);
        if (channel == null) return;

        var messageTemplate = await _templateRepository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
        if (messageTemplate is null)
        {
            messageRecord.SetResult(false, _i18n.T("MessageTemplate"));
            await _messageRecordRepository.UpdateAsync(messageRecord);
            return;
        }

        if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
        {
            messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
            await _messageRecordRepository.UpdateAsync(messageRecord);
            return;
        }

        var templateId = messageRecord.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
        if (string.IsNullOrWhiteSpace(templateId))
        {
            messageRecord.SetResult(false, "TemplateId is required");
            await _messageRecordRepository.UpdateAsync(messageRecord);
            return;
        }

        if (string.IsNullOrWhiteSpace(messageRecord.ChannelUserIdentity))
        {
            messageRecord.SetResult(false, "OpenId is required");
            await _messageRecordRepository.UpdateAsync(messageRecord);
            return;
        }

        var page = messageTemplate.MessageContent.IsJump
            ? Render(messageTemplate.MessageContent.JumpUrl, messageRecord.Variables)
            : string.Empty;
        var message = new WeixinMiniProgramSubscribeMessage(
            messageRecord.ChannelUserIdentity,
            templateId,
            page,
            BuildTemplateData(messageTemplate, messageRecord.Variables));

        using (_asyncLocal.Change(channel.GetWeixinMiniProgramOptions()))
        {
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
                _logger.LogError(ex, "RetryWeixinMiniProgramMessageEventHandler");
                messageRecord.SetResult(false, ex.Message);
            }
        }

        await _messageRecordRepository.UpdateAsync(messageRecord);
    }

    private Dictionary<string, string> BuildTemplateData(MessageTemplate messageTemplate, ExtraPropertyDictionary variables)
    {
        var convertedVariables = _messageTemplateDomainService.ConvertVariables(messageTemplate, variables);
        return convertedVariables.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? string.Empty);
    }

    private string Render(string context, ExtraPropertyDictionary variables)
    {
        foreach (var item in variables)
        {
            context = context.Replace($"{{{{{item.Key}}}}}", item.Value?.ToString() ?? string.Empty);
        }

        return context;
    }

}
