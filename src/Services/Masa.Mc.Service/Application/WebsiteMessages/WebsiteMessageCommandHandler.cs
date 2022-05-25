// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCommandHandler
{
    private readonly IWebsiteMessageRepository _repository;

    public WebsiteMessageCommandHandler(IWebsiteMessageRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task SetAllReadAsync(SetAllReadWebsiteMessageCommand command)
    {
        var queryable = await CreateFilteredQueryAsync(command.dto);
        var list = await queryable.ToListAsync();
        foreach (var item in list)
        {
            item.SetRead();
        }
        await _repository.UpdateRangeAsync(list);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteWebsiteMessageCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.WebsiteMessageId);
        if (entity == null)
            throw new UserFriendlyException("WebsiteMessage not found");
        await _repository.RemoveAsync(entity);
    }

    private async Task<Expression<Func<WebsiteMessage, bool>>> CreateFilteredPredicate(GetWebsiteMessageInputDto inputDto)
    {
        Expression<Func<WebsiteMessage, bool>> condition = channel => true;
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
        var query = await _repository.GetQueryableAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }
}