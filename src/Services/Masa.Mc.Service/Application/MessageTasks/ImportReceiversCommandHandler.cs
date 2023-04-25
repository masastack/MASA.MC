// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class ImportReceiversCommandHandler
{
    private readonly ICsvImporter _importer;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private ImportResult<dynamic> _importResult = new ImportResult<dynamic>();
    private List<MessageTaskReceiverDto> _importDtos = new List<MessageTaskReceiverDto>();
    private readonly II18n<DefaultResource> _i18n;

    public ImportReceiversCommandHandler(ICsvImporter importer
        , IMessageTemplateRepository messageTemplateRepository
        , II18n<DefaultResource> i18n)
    {
        _importer = importer;
        _messageTemplateRepository = messageTemplateRepository;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task ImportReceiversAsync(ImportReceiversCommand command)
    {
        var template = await _messageTemplateRepository.FindAsync(x => x.Id == command.Dto.MessageTemplatesId);
        var file = command.Dto;
        var stream = new MemoryStream(file.FileContent);
        command.Result = await DynamicImportMessageTaskReceiver(command.Dto.ChannelType, stream, template?.Items.ToList() ?? new());
    }

    private async Task<ImportResultDto<MessageTaskReceiverDto>> DynamicImportMessageTaskReceiver(ChannelTypes channelType, Stream stream, List<MessageTemplateItem> items)
    {
        _importResult = await GetDynamicImportDtos(channelType, stream);
        _importDtos = GetMessageTaskReceiverDtos(channelType, items);
        CheckImportDtos(channelType);
        var result = new ImportResultDto<MessageTaskReceiverDto>
        {
            HasError = _importResult.HasError,
            RowErrors = _importResult.RowErrors,
            TemplateErrors = _importResult.TemplateErrors,
            ErrorMsg = _importResult.Exception?.Message ?? string.Empty,
            Data = _importDtos
        };
        return result;
    }

    private List<MessageTaskReceiverDto> GetMessageTaskReceiverDtos(ChannelTypes channelType, List<MessageTemplateItem> items)
    {
        List<MessageTaskReceiverDto> importDtos = new();
        if (_importResult.Data == null || !_importResult.Data.Any())
        {
            _importResult.Exception = new Exception(_i18n.T("NoValidData"));
            return importDtos;
        }
        foreach (var item in _importResult.Data)
        {
            var obj = item as ExpandoObject;
            var receiver = GetMessageTaskReceiverDto(channelType, obj);
            foreach (var t in items)
            {
                receiver.Variables.TryAdd(t.Code, obj.GetOrDefault(t.Code)?.ToString() ?? string.Empty);
            }
            importDtos.Add(receiver);
        }
        return importDtos;
    }

    private string GetImporterHeaderDisplayName(Type type, string name)
    {
        var fieldAttribute = type.GetProperty(name)?.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
        if (fieldAttribute != null)
        {
            name = fieldAttribute.Name;
        }
        return name;
    }

    private async Task<ImportResult<dynamic>> GetDynamicImportDtos(ChannelTypes channelType, Stream stream)
    {
        switch (channelType)
        {
            case ChannelTypes.Sms:
                return await _importer.DynamicImport<SmsReceiverImportDto>(stream);
            case ChannelTypes.Email:
                return await _importer.DynamicImport<EmailReceiverImportDto>(stream);
            case ChannelTypes.WebsiteMessage:
                return await _importer.DynamicImport<WebsiteMessageReceiverImportDto>(stream);
            default:
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.UNKNOWN_CHANNEL_TYPE);
        }
    }

    private MessageTaskReceiverDto GetMessageTaskReceiverDto(ChannelTypes channelType, ExpandoObject obj)
    {
        var receiver = new MessageTaskReceiverDto { Type = MessageTaskReceiverTypes.User };
        switch (channelType)
        {
            case ChannelTypes.Sms:
                receiver.PhoneNumber = obj?.GetOrDefault(GetImporterHeaderDisplayName(typeof(SmsReceiverImportDto), nameof(SmsReceiverImportDto.PhoneNumber)))?.ToString() ?? string.Empty;
                break;
            case ChannelTypes.Email:
                receiver.Email = obj?.GetOrDefault(GetImporterHeaderDisplayName(typeof(EmailReceiverImportDto), nameof(EmailReceiverImportDto.Email)))?.ToString() ?? string.Empty;
                break;
            case ChannelTypes.WebsiteMessage:
                receiver.SubjectId = Guid.Empty;
                object? subjectIdObj = obj.GetOrDefault(GetImporterHeaderDisplayName(typeof(WebsiteMessageReceiverImportDto), nameof(WebsiteMessageReceiverImportDto.UserId)));
                if (Guid.TryParse(subjectIdObj?.ToString(), out var subjectId))
                {
                    receiver.SubjectId = subjectId;
                }
                break;
            default:
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.UNKNOWN_CHANNEL_TYPE);
        }
        return receiver;
    }

    private void CheckImportDtos(ChannelTypes channelType)
    {
        switch (channelType)
        {
            case ChannelTypes.Sms:
                ValidateSmsImportDtos();
                break;
            case ChannelTypes.Email:
                ValidateEmailImportDtos();
                break;
            case ChannelTypes.WebsiteMessage:
                ValidateWebsiteMessageImportDtos();
                break;
            default:
                break;
        }
    }

    private void ValidateWebsiteMessageImportDtos()
    {
        _importDtos = _importDtos.GroupBy(x => x.SubjectId).Select(x => x.First()).ToList();

        if (_importDtos.Any(x => x.SubjectId == Guid.Empty))
        {
            _importResult.Exception = new Exception(_i18n.T("ErrorUserIdFormat"));
            _importDtos = new();
        }
    }

    private void ValidateSmsImportDtos()
    {
        _importDtos = _importDtos.GroupBy(x => x.PhoneNumber).Select(x => x.First()).ToList();

        if (_importDtos.Any(x => string.IsNullOrEmpty(x.PhoneNumber)))
        {
            _importResult.Exception = new Exception(_i18n.T("PhoneCannotEmpty"));
            _importDtos = new();
        }

    }

    private void ValidateEmailImportDtos()
    {
        _importDtos = _importDtos.GroupBy(x => x.Email).Select(x => x.First()).ToList();

        if (_importDtos.Any(x => string.IsNullOrEmpty(x.Email)))
        {
            _importResult.Exception = new Exception(_i18n.T("EmailCannotEmpty"));
            _importDtos = new();
        }
    }
}
