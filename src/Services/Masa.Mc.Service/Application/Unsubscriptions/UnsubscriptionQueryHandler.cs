// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

public class UnsubscriptionQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IAuthClient _authClient;

    public UnsubscriptionQueryHandler(IMcQueryContext context, II18n<DefaultResource> i18n, IAuthClient authClient)
    {
        _context = context;
        _i18n = i18n;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetListAsync(GetUnsubscriptionListQuery query)
    {
        var input = query.Input;
        var condition = CreatePredicate(input);
        var resultList = await _context.UnsubscriptionQueries
            .GetPaginatedListAsync(condition, new()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                Sorting = new Dictionary<string, bool>
                {
                    [nameof(UnsubscriptionQueryModel.UnsubscribedAt)] = true
                }
            });

        var dtos = resultList.Result.Adapt<List<UnsubscriptionDto>>();
        await FillScopeDisplayNamesAsync(dtos);
        query.Result = new PaginatedListDto<UnsubscriptionDto>(resultList.Total, resultList.TotalPages, dtos);
    }

    [EventHandler]
    public async Task GetAsync(GetUnsubscriptionQuery query)
    {
        var entity = await _context.UnsubscriptionQueries
            .Include(x => x.Timelines)
            .FirstOrDefaultAsync(x =>
                x.Id == query.Id &&
                x.ChannelType == ChannelTypes.Sms &&
                x.ChannelProvider == (int)SmsInboundProviders.YunMas);

        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageRecord"));

        var dto = entity.Adapt<UnsubscriptionDetailDto>();
        await FillScopeDisplayNamesAsync(new List<UnsubscriptionDto> { dto });
        dto.Timelines = entity.Timelines
            .OrderBy(x => x.OccurredAt)
            .Adapt<List<UnsubscriptionTimelineDto>>();
        query.Result = dto;
    }

    private static Expression<Func<UnsubscriptionQueryModel, bool>> CreatePredicate(GetUnsubscriptionInputDto input)
    {
        var condition = CreateUnsubscriptionScopePredicate(input);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.Filter), x =>
            x.ChannelUserIdentity.Contains(input.Filter) ||
            x.Keyword.Contains(input.Filter) ||
            x.Reason.Contains(input.Filter));
        condition = condition.And(input.Source.HasValue, x => x.Source == input.Source);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Keyword == input.Keyword);
        condition = condition.And(input.Status.HasValue, x => x.Status == input.Status);
        condition = condition.And(input.StartTime.HasValue, x => x.UnsubscribedAt >= input.StartTime);
        condition = condition.And(input.EndTime.HasValue, x => x.UnsubscribedAt <= input.EndTime);
        return condition;
    }

    private static Expression<Func<UnsubscriptionQueryModel, bool>> CreateUnsubscriptionScopePredicate(GetUnsubscriptionInputDto input)
    {
        Expression<Func<UnsubscriptionQueryModel, bool>> condition = x =>
            x.ChannelType == ChannelTypes.Sms &&
            x.ChannelProvider == (int)SmsInboundProviders.YunMas;

        condition = condition.And(input.ChannelId.HasValue, x => x.ChannelId == input.ChannelId);
        condition = condition.And(input.UserId.HasValue, x => x.UserId == input.UserId);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.ChannelUserIdentity), x => x.ChannelUserIdentity.Contains(input.ChannelUserIdentity));
        condition = condition.And(!string.IsNullOrWhiteSpace(input.ScopeRefId), x => x.ScopeRefId == input.ScopeRefId);
        return condition;
    }

    private async Task FillScopeDisplayNamesAsync(List<UnsubscriptionDto> dtos)
    {
        await FillChannelDisplayNamesAsync(dtos);
        await FillUserDisplayNamesAsync(dtos);

        var templateIds = dtos
            .Where(x => x.ScopeType == UnsubscriptionScopeTypes.Template)
            .Select(x => Guid.TryParse(x.ScopeRefId, out var guid) ? guid : Guid.Empty)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (!templateIds.Any())
        {
            return;
        }

        var templateNames = await _context.MessageTemplateQueries
            .Where(x => templateIds.Contains(x.Id))
            .Select(x => new { x.Id, x.DisplayName })
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName);

        foreach (var item in dtos.Where(x => x.ScopeType == UnsubscriptionScopeTypes.Template))
        {
            var templateId = Guid.TryParse(item.ScopeRefId, out var guid) ? guid : Guid.Empty;
            if (templateNames.TryGetValue(templateId, out var displayName))
            {
                item.ScopeDisplayName = displayName;
            }
        }
    }

    private async Task FillChannelDisplayNamesAsync(List<UnsubscriptionDto> dtos)
    {
        var channelIds = dtos.Select(x => x.ChannelId).Distinct().ToList();
        if (!channelIds.Any())
        {
            return;
        }

        var channels = await _context.ChannelQueryQueries
            .Where(x => channelIds.Contains(x.Id))
            .Select(x => new { x.Id, x.DisplayName })
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName);

        foreach (var item in dtos)
        {
            if (channels.TryGetValue(item.ChannelId, out var channelDisplayName))
            {
                item.ChannelDisplayName = channelDisplayName;
            }
        }
    }

    private async Task FillUserDisplayNamesAsync(List<UnsubscriptionDto> dtos)
    {
        var userIds = dtos
            .Select(x => x.UserId)
            .Concat(dtos.Select(x => x.Modifier))
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();
        if (!userIds.Any())
        {
            return;
        }

        var users = (await _authClient.UserService.GetListByIdsAsync(userIds))
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.First().RealDisplayName ?? string.Empty);

        foreach (var item in dtos)
        {
            if (users.TryGetValue(item.UserId, out var userDisplayName))
            {
                item.UserDisplayName = userDisplayName;
            }

            if (users.TryGetValue(item.Modifier, out var modifierName))
            {
                item.ModifierName = modifierName;
            }
        }
    }
}
