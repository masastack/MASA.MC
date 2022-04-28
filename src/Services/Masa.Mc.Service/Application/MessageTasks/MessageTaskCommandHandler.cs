﻿namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly MessageTaskDomainService _domainService;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , MessageTaskDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageTaskCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (entity.IsEnabled)
            throw new UserFriendlyException("enabled status cannot be deleted");
        await _repository.RemoveAsync(entity);
    }

    [EventHandler]
    public async Task SendAsync(SendMessageTaskCommand command)
    {
        var input = command.input;
        await _domainService.SendAsync(input.Id, input.ReceiverType, ExtensionPropertyHelper.ObjMapToExtraProperty(input.Receivers), ExtensionPropertyHelper.ObjMapToExtraProperty(input.SendingRules), input.SendTime, input.Sign, input.Variables);
    }

    [EventHandler]
    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        var history = entity.Historys.FirstOrDefault(x => x.Id == command.Input.HistoryId);
        if (history == null)
            throw new UserFriendlyException("history not found");
        if (history.Status == MessageTaskHistoryStatus.Withdrawn)
            throw new UserFriendlyException("withdrawn");
        history.SetWithdraw();
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task EnabledAsync(EnabledMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        entity.SetEnabled();
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DisableAsync(DisableMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (entity.Historys.Any(x => x.Status == MessageTaskHistoryStatus.Sending))
            throw new UserFriendlyException("the task has a sending task history and cannot be disabled.");
        entity.SetDisable();
        await _repository.UpdateAsync(entity);
    }
}
