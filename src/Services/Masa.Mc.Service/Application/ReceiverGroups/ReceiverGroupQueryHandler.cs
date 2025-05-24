// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Linq;

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;
    private readonly II18n<DefaultResource> _i18n;

    public ReceiverGroupQueryHandler(IMcQueryContext context
        , IAuthClient authClient
        , II18n<DefaultResource> i18n)
    {
        _context = context;
        _authClient = authClient;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task GetAsync(GetReceiverGroupQuery query)
    {
        var entity = await _context.ReceiverGroupQueries.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == query.ReceiverGroupId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("ReceiverGroup"));

        var dto = entity.Adapt<ReceiverGroupDto>();
        await FillReceiverGroupDto(dto);

        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetReceiverGroupListQuery query)
    {
        var options = query.Input;
        var condition = await CreateFilteredPredicate(options);

        var resultList = await _context.ReceiverGroupQueries.GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ReceiverGroupQueryModel.ModificationTime)] = true
            }
        });

        var dtos = resultList.Result.Adapt<List<ReceiverGroupDto>>();
        await FillReceiverGroupDtos(dtos);

        var result = new PaginatedListDto<ReceiverGroupDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    private async Task<Expression<Func<ReceiverGroupQueryModel, bool>>> CreateFilteredPredicate(GetReceiverGroupInputDto inputDto)
    {
        Expression<Func<ReceiverGroupQueryModel, bool>> condition = channel => true;
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), r => r.DisplayName.Contains(inputDto.Filter));
        return await Task.FromResult(condition); ;
    }

    private async Task FillReceiverGroupDtos(List<ReceiverGroupDto> dtos)
    {
        var modifierUserIds = dtos.Where(x => x.Modifier != Guid.Empty).Select(x => x.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetListByIdsAsync(modifierUserIds);

        foreach (var item in dtos)
        {
            var modifier = userInfos.FirstOrDefault(x => x.Id == item.Modifier);
            item.ModifierName = modifier?.RealDisplayName ?? string.Empty;
        }
    }

    private async Task FillReceiverGroupDto(ReceiverGroupDto dto)
    {
        var userIds = dto.Items.Select(x => x.SubjectId).Append(dto.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetListByIdsAsync(userIds);

        var userInfoDict = userInfos.ToDictionary(x => x.Id, x => x);

        if (userInfoDict.TryGetValue(dto.Modifier, out var modifier))
        {
            dto.ModifierName = modifier.RealDisplayName ?? string.Empty;
        }

        foreach (var item in dto.Items)
        {
            if (userInfoDict.TryGetValue(item.SubjectId, out var user))
            {
                item.DisplayName = user.RealDisplayName;
                item.Avatar = user.Avatar;
                item.PhoneNumber = user.PhoneNumber ?? string.Empty;
                item.Email = user.Email ?? string.Empty;
            }
        }
    }
}
