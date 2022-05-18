// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Magicodes.ExporterAndImporter.Core.Models;

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _domainService;
    private readonly ICsvImporter _importer;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService domainService
        , ICsvImporter importer
        , IMessageTemplateRepository messageTemplateRepository)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _domainService = domainService;
        _importer = importer;
        _messageTemplateRepository = messageTemplateRepository;
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
        var template = await _messageTemplateRepository.FindAsync(x => x.Id == command.Dto.MessageTemplatesId);
        var file = command.Dto;
        var stream = new MemoryStream(file.FileContent);
        if (template == null)
        {
            command.Result = await ImportMessageTaskReceiver(stream);
        }
        else
        {
            command.Result = await DynamicImportMessageTaskReceiver(stream, template.Items.ToList());
        }
    }

    private async Task<ImportResultDto<MessageTaskReceiverDto>> ImportMessageTaskReceiver(Stream stream)
    {
        var import = await _importer.Import<ReceiverImportDto>(stream);
        List<MessageTaskReceiverDto> importDtos = new();
        if (import.Data != null)
        {
            importDtos = import.Data.Select(x => new MessageTaskReceiverDto
            {
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Type = MessageTaskReceiverTypes.User
            }).ToList();
        }
        var result = new ImportResultDto<MessageTaskReceiverDto>
        {
            HasError = import.HasError,
            RowErrors = import.RowErrors,
            TemplateErrors = import.TemplateErrors,
            ErrorMsg = import.Exception?.Message ?? string.Empty,
            Data = importDtos
        };
        return result;
    }

    private async Task<ImportResultDto<MessageTaskReceiverDto>> DynamicImportMessageTaskReceiver(Stream stream, List<MessageTemplateItem> items)
    {
        var dynamicImport = await _importer.DynamicImport<ReceiverImportDto>(stream);
        List<MessageTaskReceiverDto> importDtos = CheckDynamicImportDtos(dynamicImport, items);
        var result = new ImportResultDto<MessageTaskReceiverDto>
        {
            HasError = dynamicImport.HasError,
            RowErrors = dynamicImport.RowErrors,
            TemplateErrors = dynamicImport.TemplateErrors,
            ErrorMsg = dynamicImport.Exception?.Message ?? string.Empty,
            Data = importDtos
        };
        return result;
    }

    private List<MessageTaskReceiverDto> CheckDynamicImportDtos(ImportResult<dynamic> dynamicImportDtos, List<MessageTemplateItem> items)
    {
        List<MessageTaskReceiverDto> importDtos = new();
        if (dynamicImportDtos.Data == null|| !dynamicImportDtos.Data.Any())
        {
            dynamicImportDtos.Exception = new Exception("no valid data detected");
            return importDtos;
        }
        foreach (var item in dynamicImportDtos.Data)
        {
            var obj = item as ExpandoObject;
            var receiver = new MessageTaskReceiverDto
            {
                DisplayName = obj.GetOrDefault(GetImporterHeaderDisplayName(nameof(ReceiverImportDto.DisplayName)))?.ToString() ?? string.Empty,
                PhoneNumber = obj.GetOrDefault(GetImporterHeaderDisplayName(nameof(ReceiverImportDto.PhoneNumber)))?.ToString() ?? string.Empty,
                Email = obj.GetOrDefault(GetImporterHeaderDisplayName(nameof(ReceiverImportDto.Email)))?.ToString() ?? string.Empty,
                Type = MessageTaskReceiverTypes.User,
            };
            if (string.IsNullOrEmpty(receiver.PhoneNumber) && string.IsNullOrEmpty(receiver.Email))
            {
                dynamicImportDtos.Exception = new Exception("fill in at least one mobile phone number and email address");
                return new();
            }
            foreach (var t in items)
            {
                receiver.Variables.TryAdd(t.Code, obj.GetOrDefault(t.Code)?.ToString() ?? string.Empty);
            }
            importDtos.Add(receiver);
        }
        return importDtos;
    }

    private string GetImporterHeaderDisplayName(string name)
    {
        var fieldAttribute = typeof(ReceiverImportDto).GetProperty(name)?.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
        if (fieldAttribute != null)
        {
            name = fieldAttribute.Name;
        }
        return name;
    }
}
