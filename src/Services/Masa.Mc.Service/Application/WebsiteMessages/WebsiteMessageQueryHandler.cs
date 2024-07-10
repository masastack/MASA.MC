// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IUserContext _userContext;
    private readonly II18n<DefaultResource> _i18n;
    private IMultilevelCacheClient _cache;

    public WebsiteMessageQueryHandler(IMcQueryContext context
        , IUserContext userContext
        , II18n<DefaultResource> i18n
        , IMultilevelCacheClient cache)
    {
        _context = context;
        _userContext = userContext;
        _i18n = i18n;
        _cache = cache;
    }

    [EventHandler]
    public async Task GetAsync(GetWebsiteMessageQuery query)
    {
        var entity = await _context.WebsiteMessageQueries.Include(x => x.Channel).FirstOrDefaultAsync(x => x.Id == query.WebsiteMessageId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("WebsiteMessage"));

        var dto = entity.Adapt<WebsiteMessageDto>();
        await FillDetailDto(dto);
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetWebsiteMessageListQuery query)
    {
        var options = query.Input;

        if (!options.ChannelId.HasValue && !string.IsNullOrEmpty(options.ChannelCode))
        {
            options.ChannelId = (await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Code == options.ChannelCode))?.Id;
        }

        var condition = await CreateFilteredPredicate(options);
        var resultList = await _context.WebsiteMessageQueries.Include(x => x.Channel).GetPaginatedListAsync(condition, new()
        {
            Page = options.Page,
            PageSize = options.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(WebsiteMessageQueryModel.CreationTime)] = true
            }
        });
        var dtos = resultList.Result.Adapt<List<WebsiteMessageDto>>();
        FillListDtos(dtos);
        var result = new PaginatedListDto<WebsiteMessageDto>(resultList.Total, resultList.TotalPages, dtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task GetChannelListAsync(GetChannelListWebsiteMessageQuery query)
    {
        var userId = _userContext.GetUserId<Guid>();

        var set = _context.WebsiteMessageQueries.Include(x => x.Channel).AsNoTracking().Where(x => x.Channel != null && x.UserId == userId);
        var sorted = set.OrderByDescending(x => x.CreationTime);
        var list = set.Select(x => x.ChannelId)
            .Distinct()
            .SelectMany(x => sorted.Where(y => y.ChannelId == x).Take(1));

        var entities = list.OrderByDescending(x => x.CreationTime).ToList();

        var entityDtos = entities.Adapt<List<WebsiteMessageChannelDto>>();
        await FillChannelListDtos(entityDtos);
        query.Result = entityDtos;
    }

    [EventHandler]
    public async Task GetNoticeListAsync(GetNoticeListQuery query)
    {
        var userId = _userContext.GetUserId<Guid>();
        var cacheKey = CacheKeys.GET_NOTICE_LIST + "-" + userId;

        var response = _cache.Get<List<WebsiteMessageDto>>(cacheKey);
        if (response != null)
        {
            query.Result = response;
            return;
        }

        var noticeNum = query.PageSize;
        var queryable = _context.WebsiteMessageQueries.Include(x => x.Channel).Where(x => x.Channel != null && x.UserId == userId && !x.IsWithdrawn);
        var list = await queryable.Where(x => !x.IsRead).OrderByDescending(x => x.CreationTime).Take(noticeNum).ToListAsync();
        if (list.Count < noticeNum)
        {
            var surplusNum = noticeNum - list.Count;
            var surplusList = await queryable.Where(x => x.IsRead).OrderByDescending(x => x.CreationTime).Take(surplusNum).ToListAsync();
            list.AddRange(surplusList);
        }
        var dtos = list.Adapt<List<WebsiteMessageDto>>();
        FillListDtos(dtos);

        if (dtos.Any())
        {
            _cache.Set(cacheKey, dtos ?? new(), DateTimeOffset.Now.AddMinutes(10));
        }

        query.Result = dtos ?? new();
    }

    private async Task<Expression<Func<WebsiteMessageQueryModel, bool>>> CreateFilteredPredicate(GetWebsiteMessageInputDto inputDto)
    {
        var userId = _userContext.GetUserId<Guid>();
        Expression<Func<WebsiteMessageQueryModel, bool>> condition = w => w.UserId == userId && !w.IsWithdrawn;

        switch (inputDto.FilterType)
        {
            case WebsiteMessageFilterType.MessageTitle:
                condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), w => w.Title.Contains(inputDto.Filter));
                break;
            case WebsiteMessageFilterType.MessageContent:
                condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), w => w.Content.Contains(inputDto.Filter));
                break;
            default:
                condition = condition.And(!string.IsNullOrEmpty(inputDto.Filter), w => w.Title.Contains(inputDto.Filter) || w.Content.Contains(inputDto.Filter));
                break;
        }
        condition = condition.And(inputDto.ChannelId.HasValue, w => w.ChannelId == inputDto.ChannelId);
        condition = condition.And(inputDto.IsRead.HasValue, w => w.IsRead == inputDto.IsRead);
        condition = condition.And(!string.IsNullOrEmpty(inputDto.Tag), w => w.Tags.Any(x => x.Tag == inputDto.Tag));
        return await Task.FromResult(condition); ;
    }

    private async Task FillChannelListDtos(List<WebsiteMessageChannelDto> dtos)
    {
        foreach (var item in dtos)
        {
            item.NoReading = await _context.WebsiteMessageQueries.CountAsync(x => x.ChannelId == item.ChannelId && !x.IsRead && x.UserId == item.UserId);
        }
    }

    private void FillListDtos(List<WebsiteMessageDto> dtos)
    {
        foreach (var item in dtos)
        {
            item.Abstract = HtmlHelper.CutString(item.Content, 250).Trim();
        }
    }

    private async Task FillDetailDto(WebsiteMessageDto dto)
    {
        var userId = _userContext.GetUserId<Guid>();
        Expression<Func<WebsiteMessageQueryModel, bool>> condition = w => w.UserId == userId && w.ChannelId == dto.ChannelId;
        var prev = await GetPrevWebsiteMessage(dto.CreationTime, condition);
        var next = await GetNextWebsiteMessage(dto.CreationTime, condition);
        dto.PrevId = prev != null ? prev.Id : default;
        dto.NextId = next != null ? next.Id : default;
    }

    public async Task<WebsiteMessageQueryModel?> GetPrevWebsiteMessage(DateTime creationTime, Expression<Func<WebsiteMessageQueryModel, bool>> predicate)
    {
        return await _context.WebsiteMessageQueries.Where(predicate).Where(x => x.CreationTime < creationTime).OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync();
    }

    public async Task<WebsiteMessageQueryModel?> GetNextWebsiteMessage(DateTime creationTime, Expression<Func<WebsiteMessageQueryModel, bool>> predicate)
    {
        return await _context.WebsiteMessageQueries.Where(predicate).Where(x => x.CreationTime > creationTime).OrderBy(x => x.CreationTime).FirstOrDefaultAsync();
    }

    [EventHandler]
    public async Task GetListByTagAsync(GetWebsiteMessageTagsQuery query)
    {
        var channelId = (await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Code == query.ChannelCode))?.Id;
        var userId = _userContext.GetUserId<Guid>();
        var tags = query.Tags.Split(',');

        var set = _context.WebsiteMessageTagQueries.AsNoTracking().Where(x => x.UserId == userId && tags.Contains(x.Tag));

        if (channelId.HasValue)
        {
            set = set.Where(x => x.ChannelId == channelId);
        }

        var sorted = set.OrderByDescending(x => x.CreationTime);
        var tagQuery = set.Select(x => x.Tag)
            .Distinct()
            .SelectMany(x => sorted.Where(y => y.Tag == x).Take(1));

        var messageTagQuery = from tag in tagQuery
                              join message in _context.WebsiteMessageQueries on tag.WebsiteMessageId equals message.Id into messageJoined
                              from message in messageJoined.DefaultIfEmpty()
                              where message != null
                              select new WebsiteMessageTagDto
                              {
                                  Tag = tag.Tag,
                                  ChannelId = message.ChannelId,
                                  Title = message.Title,
                                  SendTime = message.SendTime,
                                  UserId = message.UserId
                              };

        var messageTags = await messageTagQuery.ToListAsync();

        foreach (var messageTag in messageTags)
        {
            var unread = _context.WebsiteMessageQueries.Include(x => x.Tags).Count(x => x.UserId == messageTag.UserId && x.ChannelId == messageTag.ChannelId && !x.IsRead && x.Tags.Any(y => y.Tag == messageTag.Tag));
            messageTag.Unread = unread;
        }

        query.Result = messageTags;
    }

    [EventHandler]
    public async Task GetUnreadAsync(GetUnreadMessageCountQuery query)
    {
        var userId = _userContext.GetUserId<Guid>();

        var channelId = (await _context.ChannelQueryQueries.FirstOrDefaultAsync(x => x.Code == query.ChannelCode))?.Id;

        if (!string.IsNullOrEmpty(query.Tag))
        {
            query.Result = await _context.WebsiteMessageQueries.Include(x => x.Tags).CountAsync(x => x.UserId == userId && x.ChannelId == channelId && !x.IsRead && x.Tags.Any(t=>t.Tag == query.Tag));
            return;
        }

        var unread = await _context.WebsiteMessageQueries.CountAsync(x => x.UserId == userId && x.ChannelId == channelId && !x.IsRead);

        query.Result = unread;
    }
}
