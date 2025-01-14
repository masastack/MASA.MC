// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public class MessageTemplateDomainService : DomainService
{
    private readonly IMessageTemplateRepository _repository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public MessageTemplateDomainService(IDomainEventBus eventBus
        , IMessageTemplateRepository repository
        , IMessageRecordRepository messageRecordRepository) : base(eventBus)
    {
        _repository = repository;
        _messageRecordRepository = messageRecordRepository;
    }

    public virtual async Task CreateAsync(MessageTemplate messageTemplate)
    {
        await ValidateTemplateAsync(messageTemplate);
        ParseTemplateItem(messageTemplate);
        await _repository.AddAsync(messageTemplate);
    }

    public virtual async Task UpdateAsync(MessageTemplate messageTemplate)
    {
        await ValidateTemplateAsync(messageTemplate, messageTemplate.Id);
        ParseTemplateItem(messageTemplate);
        await _repository.UpdateAsync(messageTemplate);
    }

    public async Task<MessageTemplate> DeleteAsync(MessageTemplate template)
    {
        if (template.IsStatic)
        {
            throw new UserFriendlyException("The template cannot be deleted");
        }

        template.Remove();

        return await _repository.RemoveAsync(template);
    }

    public async void ParseTemplateItem(MessageTemplate messageTemplate, string startstr = "{{", string endstr = "}}")
    {
        if (!string.IsNullOrEmpty(messageTemplate.TemplateId))
        {
            return;
        }

        var titleParam = UtilHelper.MidStrEx(messageTemplate.MessageContent.Title, startstr, endstr);
        var contentParam = UtilHelper.MidStrEx(messageTemplate.MessageContent.Content, startstr, endstr);
        var jumpUrlParam = UtilHelper.MidStrEx(messageTemplate.MessageContent.JumpUrl, startstr, endstr);
        var paramList = titleParam.Union(contentParam).Union(jumpUrlParam).ToList();
        messageTemplate.Items.Clear();
        foreach (var item in paramList)
        {
            messageTemplate.AddOrUpdateItem(item, string.Empty, string.Empty, string.Empty);
        }
    }

    protected async Task ValidateTemplateAsync(MessageTemplate expectedTemplate, Guid? expectedId = null)
    {
        if (await _repository.AnyAsync(d => d.Code == expectedTemplate.Code && d.Id != expectedId))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.TEMPLATE_CODE_CANNOT_REPEATED);
        }

        if (string.IsNullOrEmpty(expectedTemplate.TemplateId)) return;
        if (await _repository.AnyAsync(d => d.TemplateId == expectedTemplate.TemplateId && d.Id != expectedId))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.TEMPLATE_ID_CANNOT_REPEATED);
        }
    }

    public async Task<bool> CheckSendUpperLimitAsync(MessageTemplate messageTemplate, string channelUserIdentity)
    {
        var perDayLimit = messageTemplate.PerDayLimit;

        if (perDayLimit == 0)
        {
            return true;
        }

        var sendNum = await _messageRecordRepository.GetCountAsync(x => x.SendTime.Value.Date == DateTime.UtcNow.Date && x.ChannelUserIdentity == channelUserIdentity && x.MessageEntityId == messageTemplate.Id);
        if (sendNum >= perDayLimit)
        {
            return false;
        }

        return true;
    }

    public async Task<List<string>> CheckSendUpperLimitAsync(MessageTemplate messageTemplate, List<string> channelUserIdentitys)
    {
        var perDayLimit = messageTemplate.PerDayLimit;

        if (perDayLimit == 0)
        {
            return new();
        }

        var query = await _messageRecordRepository.GetQueryableAsync();

        return await query
            .Where(x => x.SendTime.Value.Date == DateTime.UtcNow.Date && channelUserIdentitys.Contains(x.ChannelUserIdentity) && x.MessageEntityId == messageTemplate.Id)
            .GroupBy(x=>x.ChannelUserIdentity)
            .Where(x => x.Count() >= perDayLimit)
            .Select(x=>x.Key)
            .ToListAsync();
    }

    public ExtraPropertyDictionary ConvertVariables(MessageTemplate messageTemplate, ExtraPropertyDictionary variables)
    {
        var newVariables = new ExtraPropertyDictionary();

        if (variables == null)
        {
            return newVariables;
        }

        foreach (var item in messageTemplate.Items)
        {
            var key = string.IsNullOrEmpty(item.MappingCode) ? item.Code : item.MappingCode;
            var value = variables.FirstOrDefault(x => x.Key == item.Code).Value;
            newVariables[key] = value;
        }
        return newVariables;
    }
}
