// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTaskHistorys;

public class MessageTaskHistoryQueryHandler
{
    private readonly IMessageTaskHistoryRepository _repository;

    public MessageTaskHistoryQueryHandler(IMessageTaskHistoryRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskHistoryQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTaskHistoryId);
        if (entity == null)
            throw new UserFriendlyException("messageTaskHistory not found");
        query.Result = entity.Adapt<MessageTaskHistoryDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTaskHistoryListQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "sendTime asc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageTaskHistoryDto>>();
        var result = new PaginatedListDto<MessageTaskHistoryDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTaskHistory, bool>>> CreateFilteredPredicate(GetMessageTaskHistoryInputDto inputDto)
    {
        Expression<Func<MessageTaskHistory, bool>> condition = x => !x.IsTest;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.TaskHistoryNo.Contains(inputDto.Filter));
        condition = condition.And(inputDto.MessageTaskId.HasValue, x => x.MessageTaskId == inputDto.MessageTaskId);
        condition = condition.And(inputDto.Status.HasValue, x => x.Status == inputDto.Status);
        condition = condition.And(inputDto.StartTime.HasValue, x => x.SendTime >= inputDto.StartTime);
        condition = condition.And(inputDto.EndTime.HasValue, x => x.SendTime <= inputDto.EndTime);
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTaskHistory>> CreateFilteredQueryAsync(GetMessageTaskHistoryInputDto inputDto)
    {
        var query = await _repository.GetQueryableAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }
}
