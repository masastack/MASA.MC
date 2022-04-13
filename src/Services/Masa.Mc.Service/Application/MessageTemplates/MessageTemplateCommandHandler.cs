namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateCommandHandler
{
    private readonly IMessageTemplateRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly MessageTemplateDomainService _domainService;

    public MessageTemplateCommandHandler(IMessageTemplateRepository repository, IIntegrationEventBus integrationEventBus, MessageTemplateDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageTemplateCommand createCommand)
    {
        var dto = createCommand.MessageTemplate;
        var entity = new MessageTemplate(dto.ChannelType,
                dto.ChannelId,
                dto.DisplayName,
                dto.Title,
                dto.Content,
                dto.Example,
                dto.TemplateId,
                dto.IsJump,
                dto.JumpUrl,
                dto.Sign,
                new List<MessageTemplateItem>(),
                dto.Status,
                dto.AuditStatus);
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
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        entity.SetContent(dto.ChannelType, dto.ChannelId, dto.DisplayName, dto.Title, dto.Content, dto.Example,dto.TemplateId, dto.IsJump, dto.JumpUrl, dto.Sign);
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
        if (entity == null)
            throw new UserFriendlyException("messageTemplate not found");
        await _domainService.DeleteAsync(entity);
    }
}
