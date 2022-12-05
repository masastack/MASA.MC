// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTaskHistorys;

public class MessageTaskHistoryQueryHandler
{
    private readonly IMcQueryContext _context;

    public MessageTaskHistoryQueryHandler(IMcQueryContext context)
    {
        _context = context;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskHistoryQuery query)
    {
        var entity = await _context.MessageTaskHistoryQueries.FirstOrDefaultAsync(x => x.Id == query.MessageTaskHistoryId);
        MasaArgumentException.ThrowIfNull(entity, "MessageTaskHistory");

        query.Result = entity.Adapt<MessageTaskHistoryDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetMessageTaskHistoryListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.MessageTaskHistoryQueries.GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTaskHistoryQueryModel.SendTime)] = true
            }
        });

        var dtos = resultList.Result.Adapt<List<MessageTaskHistoryDto>>();
        var result = new PaginatedListDto<MessageTaskHistoryDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTaskHistoryQueryModel, bool>>> CreateFilteredPredicate(GetMessageTaskHistoryInputDto inputDto)
    {
        Expression<Func<MessageTaskHistoryQueryModel, bool>> condition = x => !x.IsTest;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.TaskHistoryNo.Contains(inputDto.Filter));
        condition = condition.And(inputDto.MessageTaskId.HasValue, x => x.MessageTaskId == inputDto.MessageTaskId);
        condition = condition.And(inputDto.Status.HasValue, x => x.Status == inputDto.Status);
        condition = condition.And(inputDto.StartTime.HasValue, x => x.SendTime >= inputDto.StartTime);
        condition = condition.And(inputDto.EndTime.HasValue, x => x.SendTime <= inputDto.EndTime);
        return await Task.FromResult(condition);
    }
}