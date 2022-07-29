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

        return await _repository.RemoveAsync(template);
    }

    public async void ParseTemplateItem(MessageTemplate messageTemplate, string startstr = "{{", string endstr = "}}")
    {
        if (!string.IsNullOrEmpty(messageTemplate.TemplateId))
        {
            return;
        }

        var titleParam = UtilHelper.MidStrEx(messageTemplate.Title, startstr, endstr);
        var contentParam = UtilHelper.MidStrEx(messageTemplate.Content, startstr, endstr);
        var paramList = titleParam.Union(contentParam).ToList();
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
            throw new UserFriendlyException("Template code cannot be repeated");
        }

        if (string.IsNullOrEmpty(expectedTemplate.TemplateId)) return;
        if (await _repository.AnyAsync(d => d.TemplateId == expectedTemplate.TemplateId && d.Id != expectedId))
        {
            throw new UserFriendlyException("Message templateId cannot be repeated");
        }
    }


    public async Task<bool> CheckSendUpperLimitAsync(Guid messageTemplateId, long perDayLimit, Guid userId)
    {
        var sendNum = await _messageRecordRepository.GetCountAsync(x => x.SendTime.Value.Date == DateTime.Now.Date && x.UserId == userId && x.MessageEntityId == messageTemplateId);
        if (sendNum > perDayLimit)
        {
            return false;
        }
        return true;
    }

    public ExtraPropertyDictionary ConvertVariables(List<MessageTemplateItem> Items, ExtraPropertyDictionary variables)
    {
        var newVariables = new ExtraPropertyDictionary();

        if (variables == null)
        {
            return newVariables;
        }

        foreach (var item in Items)
        {
            var key = string.IsNullOrEmpty(item.MappingCode) ? item.Code : item.MappingCode;
            var value = variables.FirstOrDefault(x => x.Key == item.Code).Value;
            newVariables[key] = value;
        }
        return newVariables;
    }
}
