// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Infrastructure.EntityFrameworkCore.EntityFrameworkCore.Extensions;

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;

    public ReceiverGroupQueryHandler(IMcQueryContext context
        , IAuthClient authClient)
    {
        _context = context;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetAsync(GetReceiverGroupQuery query)
    {
        var entity = await _context.ReceiverGroupQueries.Include(x=>x.Items).FirstOrDefaultAsync(x => x.Id == query.ReceiverGroupId);

        Check.NotNull(entity, "ReceiverGroup not found");

        query.Result = entity.Adapt<ReceiverGroupDto>();
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
        var modifierUserIds = dtos.Where(x => x.Modifier != default).Select(x => x.Modifier).Distinct().ToArray();
        var userInfos = await _authClient.UserService.GetUserPortraitsAsync(modifierUserIds);
        foreach (var item in dtos)
        {
            item.ModifierName = userInfos.FirstOrDefault(x => x.Id == item.Modifier)?.DisplayName ?? string.Empty;
        }
    }
}
