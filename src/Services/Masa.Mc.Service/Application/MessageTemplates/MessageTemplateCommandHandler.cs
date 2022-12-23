// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateCommandHandler
{
    private readonly IMessageTemplateRepository _repository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly MessageTemplateDomainService _domainService;
    private readonly II18n<DefaultResource> _i18n;

    public MessageTemplateCommandHandler(IMessageTemplateRepository repository, IMessageTaskRepository messageTaskRepository, MessageTemplateDomainService domainService, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _messageTaskRepository = messageTaskRepository;
        _domainService = domainService;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageTemplateCommand createCommand)
    {
        var dto = createCommand.MessageTemplate;
        var entity = dto.Adapt<MessageTemplate>();
        foreach (var itemDto in dto.Items)
        {
            entity.AddOrUpdateItem(itemDto.Code, itemDto.MappingCode, itemDto.DisplayText, itemDto.Description);
        }
        await _domainService.CreateAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateMessageTemplateCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTemplateId);
        var dto = updateCommand.MessageTemplate;
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTemplate"));

        dto.Adapt(entity);
        foreach (var itemDto in dto.Items)
        {
            entity.AddOrUpdateItem(itemDto.Code, itemDto.MappingCode, itemDto.DisplayText, itemDto.Description);
        }
        entity.Items.RemoveAll(item => !dto.Items.Select(dtoItem => dtoItem.Code).Contains(item.Code));

        await _domainService.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageTemplateCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageTemplateId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTemplate"));

        if (await _messageTaskRepository.FindAsync(x => x.EntityType == MessageEntityTypes.Template && x.EntityId == createCommand.MessageTemplateId, false) != null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TEMPLATE_CANNOT_DELETE_BY_MESSAGE_TASK);
        }
        await _domainService.DeleteAsync(entity);
    }
}
