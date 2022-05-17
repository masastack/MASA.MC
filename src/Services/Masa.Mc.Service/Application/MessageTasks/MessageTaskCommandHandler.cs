// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _domainService;
    private readonly ICsvImporter _importer;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService domainService
        , ICsvImporter importer)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _domainService = domainService;
        _importer = importer;
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
        var inputDto = command.inputDto;
        var receivers = inputDto.Receivers.Adapt<List<MessageTaskReceiver>>();
        await _domainService.SendAsync(inputDto.Id, inputDto.ReceiverType, receivers, ExtensionPropertyHelper.ObjMapToExtraProperty(inputDto.SendRules), inputDto.SendTime, inputDto.Sign, inputDto.Variables);
    }

    [EventHandler]
    public async Task SendTestAsync(SendTestMessageTaskCommand command)
    {
        var inputDto = command.inputDto;
        var entity = await _repository.FindAsync(x => x.Id == inputDto.Id);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (entity.Channel.Type == ChannelTypes.Sms && string.IsNullOrEmpty(entity.Sign))
            throw new UserFriendlyException("please fill in the signature of the task first");
        if (entity.Variables.Any(x => string.IsNullOrEmpty(x.Value.ToString())))
            throw new UserFriendlyException("please fill in the signature template variable of the task first");
        var receivers = inputDto.Receivers.Adapt<List<MessageTaskReceiver>>();
        await _domainService.SendAsync(inputDto.Id, ReceiverTypes.Assign, receivers, new ExtraPropertyDictionary(), DateTime.UtcNow, entity.Sign, entity.Variables);
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
        if (await _messageTaskHistoryRepository.FindAsync(x => x.MessageTaskId == command.Input.MessageTaskId && x.Status == MessageTaskHistoryStatuses.Sending) != null)
            throw new UserFriendlyException("the task has a sending task history and cannot be disabled.");
        entity.SetDisable();
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task ImportReceiversAsync(ImportReceiversCommand command)
    {
        var file = command.File;
        var stream = new MemoryStream(file.FileContent);
        var import = await _importer.Import<ReceiverImportDto>(stream);
        var importDtos = import.Data?.ToList() ?? new();
        var result = new ImportResultDto<MessageTaskReceiverDto>
        {
            HasError = import.HasError,
            RowErrors = import.RowErrors,
            TemplateErrors = import.TemplateErrors,
            ErrorMsg = import.Exception?.Message ?? string.Empty,
            Data = importDtos.Select(x => new MessageTaskReceiverDto
            {
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Type = MessageTaskReceiverTypes.User
            }).ToList()
        };
        command.Result = result;
    }
}
