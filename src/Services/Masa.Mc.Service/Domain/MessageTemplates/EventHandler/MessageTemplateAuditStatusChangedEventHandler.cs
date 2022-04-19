namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class MessageTemplateAuditStatusChangedEventHandler
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateAuditStatusChangedEventHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToApproved(MessageTemplateAuditStatusChangedToApprovedDomainEvent @event)
    {
        var entity = await _repository.FindAsync(x => x.Id == @event.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatus.Adopt, @event.Remarks);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToRefuse(MessageTemplateAuditStatusChangedToRefuseDomainEvent @event)
    {
        var entity = await _repository.FindAsync(x => x.Id == @event.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatus.Fail, @event.Remarks);
        await _repository.UpdateAsync(entity);
    }
}