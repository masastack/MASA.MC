namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public MessageTaskQueryHandler(IMessageTaskRepository repository, IMessageTemplateRepository messageTemplateRepository)
    {
        _repository = repository;
        _messageTemplateRepository = messageTemplateRepository;
    }

    [EventHandler]
    public async Task GetAsync(GetMessageTaskQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
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
        await FillMessageInfoAsync(entityDtos);
        var result = new PaginatedListDto<MessageTaskDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTask, bool>>> CreateFilteredPredicate(GetMessageTaskInput input)
    {
        Expression<Func<MessageTask, bool>> condition = x => true;
        condition = condition.And(input.EntityType.HasValue, x => x.EntityType == input.EntityType);
        condition = condition.And(input.ChannelId.HasValue, x => x.ChannelId == input.ChannelId);
        condition = condition.And(input.IsEnabled.HasValue, x => x.IsEnabled == input.IsEnabled);
        if (input.TimeType == MessageTaskTimeType.ModificationTime)
        {
            condition = condition.And(input.StartTime.HasValue, x => x.ModificationTime >= input.StartTime);
            condition = condition.And(input.EndTime.HasValue, x => x.ModificationTime <= input.EndTime);
        }
        if (input.TimeType == MessageTaskTimeType.SendTime)
        {
            condition = condition.And(input.StartTime.HasValue, x => x.SendTime >= input.StartTime);
            condition = condition.And(input.EndTime.HasValue, x => x.SendTime <= input.EndTime);
        }
        return await Task.FromResult(condition); ;
    }

    private async Task<IQueryable<MessageTask>> CreateFilteredQueryAsync(GetMessageTaskInput input)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(input);
        return query.Where(condition);
    }

    private async Task FillMessageInfoAsync(List<MessageTaskDto> dtos)
    {
        var group = dtos.GroupBy(x => x.EntityType);
        foreach (var item in group)
        {
            var entityIds = item.Select(x => x.EntityId).ToList();
            switch (item.Key)
            {
                case MessageEntityType.Ordinary:
                    break;
                case MessageEntityType.Template:
                    await FillTemplateMessageInfoAsync(dtos, entityIds);
                    break;
                default:
                    break;
            }
        }
    }

    private async Task FillTemplateMessageInfoAsync(List<MessageTaskDto> dtos, List<Guid> entityIds)
    {
        var list = await _messageTemplateRepository.GetListAsync(x => entityIds.Contains(x.Id));
        foreach (var item in dtos)
        {
            var info = list.FirstOrDefault(x => x.Id == item.EntityId);
            if (info == null) continue;
            item.MessageInfo = new MessageInfoDto(string.IsNullOrEmpty(info.Title) ? info.DisplayName : info.Title, info.Content);
        }
    }
}
