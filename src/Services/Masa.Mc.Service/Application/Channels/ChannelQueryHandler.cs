// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels;

public class ChannelQueryHandler
{
    private readonly IChannelRepository _repository;
    private readonly IAuthClient _authClient;

    public ChannelQueryHandler(IChannelRepository repository
        , IAuthClient authClient)
    {
        _repository = repository;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetAsync(GetChannelQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        query.Result = entity.Adapt<ChannelDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetChannelListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _repository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Channel.ModificationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<ChannelDto>>();
        await FillChannelDtos(dtos);
        var result = new PaginatedListDto<ChannelDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task FindByCodeAsync(FindChannelByCodeQuery query)
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

    private async Task<Expression<Func<Channel, bool>>> CreateFilteredPredicate(GetChannelInputDto inputDto)
    {
        Expression<Func<Channel, bool>> condition = channel => true;
        condition = condition.And(inputDto.Type.HasValue, channel => channel.Type == inputDto.Type);
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), channel => channel.DisplayName.Contains(inputDto.Filter) || channel.Code.Contains(inputDto.Filter));
        condition = condition.And(!string.IsNullOrEmpty(inputDto.DisplayName), channel => channel.DisplayName.Contains(inputDto.DisplayName));
        return await Task.FromResult(condition); ;
    }

    private async Task FillChannelDtos(List<ChannelDto> dtos)
    {
        var modifierUserIds = dtos.Where(x => x.Modifier != default).Select(x => x.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetUserPortraitsAsync(modifierUserIds);
        foreach (var item in dtos)
        {
            item.ModifierName = userInfos.FirstOrDefault(x => x.Id == item.Modifier)?.DisplayName ?? string.Empty;
        }
    }
}
