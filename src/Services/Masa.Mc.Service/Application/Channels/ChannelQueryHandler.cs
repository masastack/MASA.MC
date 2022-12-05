// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels;

public class ChannelQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;

    public ChannelQueryHandler(IMcQueryContext context
        , IAuthClient authClient)
    {
        _context = context;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetAsync(GetChannelQuery query)
    {
        var entity = await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Id == query.ChannelId);
        MasaArgumentException.ThrowIfNull(entity, "Channel");

        query.Result = entity.Adapt<ChannelDto>();
    }

    [EventHandler]
    public async Task GetListAsync(GetChannelListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.ChannelQueryQueries.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ChannelQueryModel.ModificationTime)] = true
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
        var entity = await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Code == query.Code);
        MasaArgumentException.ThrowIfNull(entity, "Channel");

        query.Result = entity.Adapt<ChannelDto>();
    }

    [EventHandler]
    public async Task GetListByTypeAsync(GetListByTypeQuery query)
    {
        var list = await _context.ChannelQueryQueries.Where(x => x.Type == query.Type).ToListAsync();
        query.Result = list.Adapt<List<ChannelDto>>();
    }

    private async Task<Expression<Func<ChannelQueryModel, bool>>> CreateFilteredPredicate(GetChannelInputDto inputDto)
    {
        Expression<Func<ChannelQueryModel, bool>> condition = channel => true;
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
