// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

public class UnsubscriptionTimelineQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IAuthClient _authClient;

    public UnsubscriptionTimelineQueryHandler(IMcQueryContext context, IAuthClient authClient)
    {
        _context = context;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task GetHistoryListAsync(GetUnsubscriptionHistoryListQuery query)
    {
        var input = query.Input;
        var unsubscriptionPredicate = CreateUnsubscriptionScopePredicate(input);
        var timelinePredicate = CreateTimelinePredicate(input);
        var historyQuery =
            from timeline in _context.UnsubscriptionTimelineQueries.Where(timelinePredicate)
            join unsubscription in _context.UnsubscriptionQueries.Where(unsubscriptionPredicate)
                on timeline.UnsubscriptionId equals unsubscription.Id
            select new UnsubscriptionHistoryItem
            {
                TimelineId = timeline.Id,
                UnsubscriptionId = unsubscription.Id,
                UserId = unsubscription.UserId,
                ChannelUserIdentity = unsubscription.ChannelUserIdentity,
                ChannelId = unsubscription.ChannelId,
                ScopeType = unsubscription.ScopeType,
                ScopeRefId = unsubscription.ScopeRefId,
                Source = unsubscription.Source,
                Action = timeline.Action,
                Status = unsubscription.Status,
                Keyword = unsubscription.Keyword,
                Detail = timeline.Detail,
                OccurredAt = timeline.OccurredAt,
                Operator = timeline.Creator == Guid.Empty ? unsubscription.Modifier : timeline.Creator
            };

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            var filter = input.Filter;
            historyQuery = historyQuery.Where(x =>
                x.ChannelUserIdentity.Contains(filter) ||
                x.Keyword.Contains(filter) ||
                x.Detail.Contains(filter));
        }

        var resultList = await historyQuery.GetPaginatedListAsync(x => true, new()
        {
            Page = input.Page,
            PageSize = input.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(UnsubscriptionHistoryItem.OccurredAt)] = true
            }
        });

        var historyItems = resultList.Result.ToList();
        var dtos = historyItems.Select(MapHistoryToDto).ToList();
        await FillHistoryDisplayNamesAsync(historyItems, dtos);
        query.Result = new PaginatedListDto<UnsubscriptionHistoryDto>(resultList.Total, resultList.TotalPages, dtos);
    }

    private static Expression<Func<UnsubscriptionQueryModel, bool>> CreateUnsubscriptionScopePredicate(GetUnsubscriptionHistoryInputDto input)
    {
        Expression<Func<UnsubscriptionQueryModel, bool>> condition = x =>
            x.ChannelType == ChannelTypes.Sms &&
            x.ChannelProvider == (int)SmsInboundProviders.YunMas;

        condition = condition.And(input.ChannelId.HasValue, x => x.ChannelId == input.ChannelId);
        condition = condition.And(input.UserId.HasValue, x => x.UserId == input.UserId);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.ChannelUserIdentity), x => x.ChannelUserIdentity.Contains(input.ChannelUserIdentity));
        condition = condition.And(!string.IsNullOrWhiteSpace(input.ScopeRefId), x => x.ScopeRefId == input.ScopeRefId);
        condition = condition.And(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Keyword.Contains(input.Keyword));
        condition = condition.And(input.Source.HasValue, x => x.Source == input.Source);
        return condition;
    }

    private static Expression<Func<UnsubscriptionTimelineQueryModel, bool>> CreateTimelinePredicate(GetUnsubscriptionHistoryInputDto input)
    {
        Expression<Func<UnsubscriptionTimelineQueryModel, bool>> condition = _ => true;
        condition = condition.And(input.StartTime.HasValue, x => x.OccurredAt >= input.StartTime);
        condition = condition.And(input.EndTime.HasValue, x => x.OccurredAt <= input.EndTime);
        condition = condition.And(input.Action.HasValue, x => x.Action == input.Action);

        return condition;
    }

    private static UnsubscriptionHistoryDto MapHistoryToDto(UnsubscriptionHistoryItem item)
    {
        return new UnsubscriptionHistoryDto
        {
            Id = item.TimelineId,
            UnsubscriptionId = item.UnsubscriptionId,
            UserDisplayName = string.Empty,
            ChannelUserIdentity = item.ChannelUserIdentity,
            Source = item.Source,
            Action = item.Action,
            Status = item.Status,
            Keyword = item.Keyword,
            Detail = item.Detail,
            OccurredAt = item.OccurredAt,
            OperatorName = string.Empty
        };
    }

    private async Task FillHistoryDisplayNamesAsync(List<UnsubscriptionHistoryItem> items, List<UnsubscriptionHistoryDto> dtos)
    {
        var dtoByTimelineId = dtos.ToDictionary(x => x.Id);

        var channelIds = items.Select(x => x.ChannelId).Distinct().ToList();
        var channels = channelIds.Any()
            ? await _context.ChannelQueryQueries
                .Where(x => channelIds.Contains(x.Id))
                .Select(x => new { x.Id, x.DisplayName })
                .ToDictionaryAsync(x => x.Id, x => x.DisplayName)
            : new Dictionary<Guid, string>();

        var templateIds = items
            .Where(x => x.ScopeType == UnsubscriptionScopeTypes.Template)
            .Select(x => Guid.TryParse(x.ScopeRefId, out var templateId) ? templateId : Guid.Empty)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();
        var templateNames = templateIds.Any()
            ? await _context.MessageTemplateQueries
                .Where(x => templateIds.Contains(x.Id))
                .Select(x => new { x.Id, x.DisplayName })
                .ToDictionaryAsync(x => x.Id, x => x.DisplayName)
            : new Dictionary<Guid, string>();

        var userIds = items
            .Select(x => x.UserId)
            .Concat(items.Select(x => x.Operator))
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();
        var userNames = userIds.Any()
            ? (await _authClient.UserService.GetListByIdsAsync(userIds))
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First().RealDisplayName ?? string.Empty)
            : new Dictionary<Guid, string>();

        foreach (var item in items)
        {
            if (!dtoByTimelineId.TryGetValue(item.TimelineId, out var dto))
            {
                continue;
            }

            if (channels.TryGetValue(item.ChannelId, out var channelDisplayName))
            {
                dto.ChannelDisplayName = channelDisplayName;
            }

            if (item.ScopeType == UnsubscriptionScopeTypes.Template &&
                Guid.TryParse(item.ScopeRefId, out var templateId) &&
                templateNames.TryGetValue(templateId, out var scopeDisplayName))
            {
                dto.ScopeDisplayName = scopeDisplayName;
            }

            if (userNames.TryGetValue(item.UserId, out var userDisplayName))
            {
                dto.UserDisplayName = userDisplayName;
            }

            if (userNames.TryGetValue(item.Operator, out var operatorName))
            {
                dto.OperatorName = operatorName;
            }
        }
    }

}
