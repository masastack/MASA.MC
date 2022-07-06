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
        await ValidateTemplateAsync(messageTemplate.TemplateId);
        if (!messageTemplate.Items.Any()) ParseTemplateItem(messageTemplate);
        await _repository.AddAsync(messageTemplate);
    }

    public virtual async Task UpdateAsync(MessageTemplate messageTemplate)
    {
        await ValidateTemplateAsync(messageTemplate.TemplateId, messageTemplate.Id);
        if (!messageTemplate.Items.Any()) ParseTemplateItem(messageTemplate);
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

    public void ParseTemplateItem(MessageTemplate messageTemplate, string startstr = "{{", string endstr = "}}")
    {
        var titleParam = UtilHelper.MidStrEx(messageTemplate.Title, startstr, endstr);
        var contentParam = UtilHelper.MidStrEx(messageTemplate.Content, startstr, endstr);
        var paramList = titleParam.Union(contentParam).ToList();
        foreach (var item in paramList)
        {
            messageTemplate.AddOrUpdateItem(item, string.Empty, string.Empty, string.Empty);
        }
    }

    protected async Task ValidateTemplateAsync(string templateId, Guid? expectedId = null)
    {
        if (string.IsNullOrEmpty(templateId)) return;
        var template = await _repository.FindAsync(d => d.TemplateId == templateId);
        if (template != null && template.Id != expectedId)
        {
            throw new UserFriendlyException("Message templateId cannot be repeated");
        }
    }

    public async Task<bool> CheckSendUpperLimitAsync(long perDayLimit, Guid userId)
    {
        var sendNum = await _messageRecordRepository.GetCountAsync(x => x.SendTime.Value.Date == DateTime.Now.Date && x.UserId == userId);
        if (sendNum > perDayLimit)
        {
            return false;
        }
        return true;
    }
}
