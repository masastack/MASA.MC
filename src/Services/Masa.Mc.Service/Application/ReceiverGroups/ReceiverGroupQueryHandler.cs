// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupQueryHandler
{
    private readonly IReceiverGroupRepository _repository;

    public ReceiverGroupQueryHandler(IReceiverGroupRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAsync(GetReceiverGroupQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.ReceiverGroupId);
        if (entity == null)
            throw new UserFriendlyException("receiverGroup not found");
        query.Result = entity.Adapt<ReceiverGroupDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListReceiverGroupQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<ReceiverGroupDto>>();
        var result = new PaginatedListDto<ReceiverGroupDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<ReceiverGroup, bool>>> CreateFilteredPredicate(GetReceiverGroupInputDto inputDto)
    {
        Expression<Func<ReceiverGroup, bool>> condition = channel => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), channel => channel.DisplayName.Contains(inputDto.Filter));
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<ReceiverGroup>> CreateFilteredQueryAsync(GetReceiverGroupInputDto inputDto)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }
}
