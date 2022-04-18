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
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ReceiverGroup.CreationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<ReceiverGroupDto>>();
        var result = new PaginatedListDto<ReceiverGroupDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<ReceiverGroup, bool>>> CreateFilteredPredicate(GetReceiverGroupInput input)
    {
        Expression<Func<ReceiverGroup, bool>> condition = channel => true;
        condition = condition.And(!string.IsNullOrEmpty(input.Filter), channel => channel.DisplayName.Contains(input.Filter));
        return await Task.FromResult(condition); ;
    }
}
