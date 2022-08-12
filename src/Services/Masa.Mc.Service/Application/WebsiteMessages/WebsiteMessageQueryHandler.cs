// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageQueryHandler
{
    private readonly IWebsiteMessageRepository _repository;
    private readonly IChannelRepository _channelRepository;
    private readonly IUserContext _userContext;

    public WebsiteMessageQueryHandler(IWebsiteMessageRepository repository
        , IChannelRepository channelRepository
        , IUserContext userContext)
    {
        _repository = repository;
        _channelRepository = channelRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task GetAsync(GetWebsiteMessageQuery query)
    {
        var entity = await _repository.FindAsync(x => x.Id == query.WebsiteMessageId);
        if (entity == null)
            throw new UserFriendlyException("WebsiteMessage not found");
        var dto = entity.Adapt<WebsiteMessageDto>();
        await FillDetailDto(dto);
        query.Result = dto;
    }

    [EventHandler]
    public async Task GetListAsync(GetWebsiteMessageListQuery query)
    {
        var options = query.Input;
        var queryable = await CreateFilteredQueryAsync(options);
        var totalCount = await queryable.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (decimal)options.PageSize);
        if (string.IsNullOrEmpty(options.Sorting)) options.Sorting = "creationTime desc";
        queryable = queryable.OrderBy(options.Sorting).PageBy(options.Page, options.PageSize);
        var entities = await queryable.ToListAsync();
        var entityDtos = entities.Adapt<List<WebsiteMessageDto>>();
        FillListDtos(entityDtos);
        var result = new PaginatedListDto<WebsiteMessageDto>(totalCount, totalPages, entityDtos);
        query.Result = result;
    }

    [EventHandler]
    public async Task GetChannelListAsync(GetChannelListWebsiteMessageQuery query)
    {
        var userId = _userContext.GetUserId<Guid>();
        var entities = await _repository.GetChannelListAsync(userId);
        var entityDtos = entities.Adapt<List<WebsiteMessageChannelDto>>();
        await FillChannelListDtos(entityDtos);
        query.Result = entityDtos;
    }

    [EventHandler]
    public async Task GetNoticeListAsync(GetNoticeListQuery query)
    {
        var noticeNum = query.PageSize;
        var queryable = await _repository.WithDetailsAsync();
        var userId = _userContext.GetUserId<Guid>();
        queryable = queryable.Where(x => x.UserId == userId && !x.IsWithdrawn);
        var list = queryable.Where(x => !x.IsRead).OrderByDescending(x => x.CreationTime).Take(noticeNum).ToList();
        if (list.Count < noticeNum)
        {
            var surplusNum = noticeNum - list.Count;
            var surplusList = queryable.Where(x => x.IsRead).OrderByDescending(x => x.CreationTime).Take(surplusNum).ToList();
            list.AddRange(surplusList);
        }
        var dtos = list.Adapt<List<WebsiteMessageDto>>();
        FillListDtos(dtos);
        query.Result = dtos;
    }

    private async Task<Expression<Func<WebsiteMessage, bool>>> CreateFilteredPredicate(GetWebsiteMessageInputDto inputDto)
    {
        var userId = _userContext.GetUserId<Guid>();
        Expression<Func<WebsiteMessage, bool>> condition = w => w.UserId == userId && !w.IsWithdrawn;
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

    private async Task<IQueryable<WebsiteMessage>> CreateFilteredQueryAsync(GetWebsiteMessageInputDto inputDto)
    {
        var query = await _repository.WithDetailsAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }

    private async Task FillChannelListDtos(List<WebsiteMessageChannelDto> dtos)
    {
        var channeIds = dtos.Select(d => d.ChannelId).ToList();
        var channeList = await _channelRepository.GetListAsync(x => channeIds.Contains(x.Id));
        foreach (var item in dtos)
        {
            item.NoReading = await _repository.GetCountAsync(x => x.ChannelId == item.ChannelId && !x.IsRead && x.UserId == item.UserId);
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
        Expression<Func<WebsiteMessage, bool>> condition = w => w.UserId == userId && w.ChannelId == dto.ChannelId;
        var prev = await GetPrevWebsiteMessage(dto.CreationTime, condition);
        var next = await GetNextWebsiteMessage(dto.CreationTime, condition);
        dto.PrevId = prev != null ? prev.Id : default;
        dto.NextId = next != null ? next.Id : default;
    }

    public async Task<WebsiteMessage?> GetPrevWebsiteMessage(DateTime creationTime, Expression<Func<WebsiteMessage, bool>> predicate)
    {
        var query = await _repository.GetQueryableAsync()!;
        return query.Where(predicate).Where(x => x.CreationTime < creationTime).OrderByDescending(x => x.CreationTime).FirstOrDefault();
    }

    public async Task<WebsiteMessage?> GetNextWebsiteMessage(DateTime creationTime, Expression<Func<WebsiteMessage, bool>> predicate)
    {
        var query = await _repository.GetQueryableAsync()!;
        return query.Where(predicate).Where(x => x.CreationTime > creationTime).OrderBy(x => x.CreationTime).FirstOrDefault();
    }
}
