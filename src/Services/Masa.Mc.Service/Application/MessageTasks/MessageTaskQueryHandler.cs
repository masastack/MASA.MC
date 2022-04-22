namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskQueryHandler
{
    private readonly IMessageTaskRepository _repository;

    public MessageTaskQueryHandler(IMessageTaskRepository repository)
    {
        _repository = repository;
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
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(MessageTask.ModificationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<MessageTaskDto>>();
        var result = new PaginatedListDto<MessageTaskDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<MessageTask, bool>>> CreateFilteredPredicate(GetMessageTaskInput input)
    {
        Expression<Func<MessageTask, bool>> condition = channel => true;
        condition = condition.And(input.EntityType.HasValue, channel => channel.EntityType == input.EntityType);
        return await Task.FromResult(condition); ;
    }
}
