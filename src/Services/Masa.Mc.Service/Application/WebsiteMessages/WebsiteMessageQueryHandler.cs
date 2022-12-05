// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageQueryHandler
{
    private readonly IMcQueryContext _context;
    private readonly IUserContext _userContext;

    public WebsiteMessageQueryHandler(IMcQueryContext context
        , IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task GetAsync(GetWebsiteMessageQuery query)
    {
        var entity = await _context.WebsiteMessageQueries.Include(x => x.Channel).FirstOrDefaultAsync(x => x.Id == query.WebsiteMessageId);
        MasaArgumentException.ThrowIfNull(entity, "WebsiteMessage");

        var dto = entity.Adapt<WebsiteMessageDto>();
        await FillDetailDto(dto);
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetWebsiteMessageListQuery query)
    {
        var options = query.Input;
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

        var set = _context.WebsiteMessageQueries.AsNoTracking().Where(x => x.UserId == userId);
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
        var noticeNum = query.PageSize;
        var queryable = _context.WebsiteMessageQueries.Include(x => x.Channel).Where(x => x.UserId == userId && !x.IsWithdrawn);
        var list = await queryable.Where(x => !x.IsRead).OrderByDescending(x => x.CreationTime).Take(noticeNum).ToListAsync();
        if (list.Count < noticeNum)
        {
            var surplusNum = noticeNum - list.Count;
            var surplusList = await queryable.Where(x => x.IsRead).OrderByDescending(x => x.CreationTime).Take(surplusNum).ToListAsync();
            list.AddRange(surplusList);
        }
        var dtos = list.Adapt<List<WebsiteMessageDto>>();
        FillListDtos(dtos);
        query.Result = dtos;
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
        return await Task.FromResult(condition); ;
    }

    private async Task FillChannelListDtos(List<WebsiteMessageChannelDto> dtos)
    {
        var channeIds = dtos.Select(d => d.ChannelId).ToList();
        var channeList = await _context.ChannelQueryQueries.Where(x => channeIds.Contains(x.Id)).ToListAsync();
        foreach (var item in dtos)
        {
            item.NoReading = await _context.WebsiteMessageQueries.CountAsync(x => x.ChannelId == item.ChannelId && !x.IsRead && x.UserId == item.UserId);
            var channel = channeList.FirstOrDefault(x => x.Id == item.ChannelId);
            if (channel != null) item.Channel = channel.Adapt<ChannelDto>();
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
}
