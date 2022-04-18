namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateAuditStatusChangedEventHandler
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateAuditStatusChangedEventHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToApproved(MessageTemplateAuditStatusChangedToApprovedEvent integrationEvent)
    {
        var entity = await _repository.FindAsync(x => x.Id == integrationEvent.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatus.Adopt, integrationEvent.Remarks);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToRefuse(MessageTemplateAuditStatusChangedToRefuseEvent integrationEvent)
    {
        var entity = await _repository.FindAsync(x => x.Id == integrationEvent.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatus.Fail, integrationEvent.Remarks);
        await _repository.UpdateAsync(entity);
    }
}
