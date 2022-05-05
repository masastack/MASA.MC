// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly ICsvImporter _csvImporter;

    public MessageTaskQueryHandler(IMessageTaskRepository repository
        , ICsvImporter csvImporter)
    {
        _repository = repository;
        _csvImporter = csvImporter;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        query.Result = entity.Adapt<MessageTaskDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageTaskQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageTaskDto>>();
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
    public async Task GenerateImportTemplateAsync(GenerateImportTemplateQuery query)
    {
        var result = await _csvImporter.GenerateTemplateBytes<ReceiverImportDto>();
        query.Result = result;
    }
}
