// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class MessageRecordQueryHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;

    public MessageRecordQueryHandler(IMessageRecordRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageRecordQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageRecordId);
        if (entity == null)
            throw new UserFriendlyException("messageRecord not found");
        query.Result = entity.Adapt<MessageRecordDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListMessageRecordQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "modificationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<MessageRecordDto>>();
        await FillMessageRecordDtos(entityDtos);
        var result = new PaginatedListDto<MessageRecordDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageRecord, bool>>> CreateFilteredPredicate(GetMessageRecordInputDto inputDto)
    {
        Expression<Func<MessageRecord, bool>> condition = x => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), x => x.DisplayName.Contains(inputDto.Filter));
        condition = condition.And(inputDto.MessageTaskHistoryId.HasValue, m => m.MessageTaskHistoryId == inputDto.MessageTaskHistoryId);
        condition = condition.And(inputDto.ChannelId.HasValue, m => m.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.Success.HasValue, m => m.Success == inputDto.Success);
        condition = condition.And(inputDto.UserId.HasValue, m => m.UserId == inputDto.UserId);
        if (inputDto.TimeType == MessageRecordTimeTypes.ExpectSendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, m => m.ExpectSendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, m => m.ExpectSendTime <= inputDto.EndTime);
        }
        if (inputDto.TimeType == MessageRecordTimeTypes.SendTime)
        {
            condition = condition.And(inputDto.StartTime.HasValue, m => m.SendTime >= inputDto.StartTime);
            condition = condition.And(inputDto.EndTime.HasValue, m => m.SendTime <= inputDto.EndTime);
        }
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageRecord>> CreateFilteredQueryAsync(GetMessageRecordInputDto inputDto)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }

    private async Task FillMessageRecordDtos(List<MessageRecordDto> dtos)
    {
        var messageTaskHistoryIds = dtos.Select(x => x.MessageTaskHistoryId).ToList();
        var messageTaskHistoryList = await _messageTaskHistoryRepository.GetListAsync(x => messageTaskHistoryIds.Contains(x.Id));
        foreach (var item in dtos)
        {
            var messageTaskHistory = messageTaskHistoryList.FirstOrDefault(x => x.Id == item.MessageTaskHistoryId);
            if (messageTaskHistory != null) item.MessageTaskHistoryNo = messageTaskHistory.TaskHistoryNo;
        }
    }
}
