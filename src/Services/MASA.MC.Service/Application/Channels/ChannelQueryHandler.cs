namespace MASA.MC.Service.Admin.Application.Channels;

public class ChannelQueryHandler
{
    private readonly IChannelRepository _repository;

    public ChannelQueryHandler(IChannelRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAsync(GetChannelQuery query)
    {
        var entity = await _repository.FindAsync(x=>x.Id==query.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        query.Result = entity.Adapt<ChannelDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetListChannelQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Channel.CreationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<ChannelDto>>();
        var result = new PaginatedListDto<ChannelDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task FindByCodeAsync(FindByCodeChannelQuery query)
    {
        var entity = await _repository.FindAsync(d => d.Code == query.Code);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        query.Result = entity.Adapt<ChannelDto>();
    }

    [EventHandler]
    public async Task GetListByTypeAsync(GetListByTypeQuery query)
    {
        var list = await _repository.GetListAsync(d => d.Type == query.Type);
        query.Result = list.Adapt<List<ChannelDto>>();
    }

    private async Task<Expression<Func<Channel, bool>>> CreateFilteredPredicate(GetChannelInput input)
    {
        Expression<Func<Channel, bool>> condition = channel => true;
        if (input.Type.HasValue)
            condition = condition.And(channel => channel.Type == input.Type);
        if (!string.IsNullOrEmpty(input.Filter))
            condition = condition.And(channel => channel.DisplayName.Contains(input.Filter)|| channel.Code.Contains(input.Filter));
        if (!string.IsNullOrEmpty(input.DisplayName))
            condition = condition.And(channel => channel.DisplayName.Contains(input.DisplayName));
        return await Task.FromResult(condition); ;
    }
}
