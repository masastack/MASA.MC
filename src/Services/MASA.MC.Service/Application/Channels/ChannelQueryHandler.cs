using AutoMapper;
using MASA.MC.Contracts.Admin.Dtos;
using MASA.MC.Contracts.Admin.Dtos.Channels;
using MASA.MC.Service.Admin.Application.Channels.Queries;
using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Channels.Repositories;
using System.Linq.Expressions;

namespace MASA.MC.Service.Admin.Application.Channels
{
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
            var entity = await _repository.FindAsync(query.ChannelId);
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
                    [nameof(Channel.CreationTime)] = false
                }
            });
            var dtos = _mapper.Map<List<ChannelDto>>(resultList.Result).ToList();
            var result = new PaginatedListDto<ChannelDto>(resultList.Total, resultList.TotalPages, dtos);
            query.Result = result;
        }

        private async Task<Expression<Func<Channel, bool>>> CreateFilteredPredicate(GetChannelInput Input)
        {
            Expression<Func<Channel, bool>> condition = channel => true;
            if (Input.Type > 0)
                condition = condition.And(channel => channel.Type == Input.Type);
            return await Task.FromResult(condition); ;
        }
    }
}
