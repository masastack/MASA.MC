// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly ICsvExporter _exporter;
    private readonly MessageTaskDomainService _domainService;

    public MessageTaskQueryHandler(IMessageTaskRepository repository
        , IMessageTemplateRepository messageTemplateRepository
        , ICsvExporter exporter
        , MessageTaskDomainService domainService)
    {
        _repository = repository;
        _messageTemplateRepository = messageTemplateRepository;
        _exporter = exporter;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        query.Result = entity.Adapt<MessageTaskDto>();
        query.Result.SendRules.IsTiming= entity.SendRules.IsTiming;
        query.Result.SendRules.IsSendingInterval = entity.SendRules.IsSendingInterval;
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTaskListQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageTaskDto>>();
        await FillMessageTaskListDtos(entityDtos);
        var result = new PaginatedListDto<MessageTaskDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTask, bool>>> CreateFilteredPredicate(GetMessageTaskInputDto inputDto)
    {
        Expression<Func<MessageTask, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.Contains(inputDto.Filter));
        condition = condition.And(inputDto.EntityType.HasValue, x => x.EntityType == inputDto.EntityType);
        condition = condition.And(inputDto.ChannelId.HasValue, x => x.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.IsEnabled.HasValue, x => x.IsEnabled == inputDto.IsEnabled);
        condition = condition.And(inputDto.IsDraft.HasValue, x => x.IsDraft == inputDto.IsDraft);
        if (inputDto.TimeType == MessageTaskTimeTypes.ModificationTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, x => x.ModificationTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, x => x.ModificationTime <= inputDto.EndTime);
        }
        if (inputDto.TimeType == MessageTaskTimeTypes.SendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, x => x.SendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, x => x.SendTime <= inputDto.EndTime);
        }
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTask>> CreateFilteredQueryAsync(GetMessageTaskInputDto inputDto)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }

    [EventHandler]
    public async Task GenerateImportTemplateAsync(GenerateReceiverImportTemplateQuery query)
    {
        var template = await _messageTemplateRepository.FindAsync(x => x.Id == query.MessageTemplateId);
        var record = new ExpandoObject();
        var properties = typeof(ReceiverImportDto).GetProperties();
        foreach (var prop in properties)
        {
            var name = prop.Name;
            var importAttribute = prop.GetCustomAttribute<Magicodes.ExporterAndImporter.Core.ImporterHeaderAttribute>();
            if (importAttribute != null)
            {
                name = importAttribute.Name ?? prop.GetDisplayName() ?? prop.Name;
            }
            record.TryAdd(name, string.Empty);
        }
        if (template != null)
        {
            foreach (var item in template.Items)
            {
                record.TryAdd(item.Code, string.Empty);
            }
        }
        var result = await _exporter.ExportDynamicHeaderAsByteArray(record);
        query.Result = result;
    }

    private async Task FillMessageTaskListDtos(List<MessageTaskDto> dtos)
    {
        foreach (var item in dtos)
        {
            var messageData = await _domainService.GetMessageDataAsync(item.EntityType, item.EntityId, item.Variables);
            item.Content = HtmlHelper.CutString(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)),280);
        }
    }
}
