namespace MASA.MC.Service.Admin.Application.Channels;

public class ChannelQueryHandler
{
    private readonly IChannelRepository _repository;
    private readonly IMapper _mapper;

    public ChannelQueryHandler(IChannelRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [EventHandler]
    public async Task GetAsync(GetChannelQuery query)
    {
        var entity = await _repository.FindAsync(x=>x.Id==query.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        query.Result = _mapper.Map<ChannelDto>(entity);
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
        var dtos = _mapper.Map<List<ChannelDto>>(resultList.Result).ToList();
        var result = new PaginatedListDto<ChannelDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task FindByCodeAsync(FindByCodeChannelQuery query)
    {
        var entity = await _repository.FindAsync(d => d.Code == query.Code);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        query.Result = _mapper.Map<ChannelDto>(entity);
    }

    private async Task<Expression<Func<Channel, bool>>> CreateFilteredPredicate(GetChannelInput input)
    {
        Expression<Func<Channel, bool>> condition = channel => true;
        if (input.Type.HasValue)
            condition = condition.And(channel => channel.Type == input.Type);
        if (!string.IsNullOrEmpty(input.DisplayName))
            condition = condition.And(channel => channel.DisplayName.Contains(input.DisplayName));
        return await Task.FromResult(condition); ;
    }
}
