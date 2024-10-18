// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Service.Admin.Application.MessageRecords.Jobs;

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCommandHandler
{
    private readonly IWebsiteMessageRepository _repository;
    private readonly IUserContext _userContext;
    private readonly II18n<DefaultResource> _i18n;

    public WebsiteMessageCommandHandler(IWebsiteMessageRepository repository, IUserContext userContext, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _userContext = userContext;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task SetAllReadAsync(ReadAllWebsiteMessageCommand command)
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
    public async Task ReadAsync(ReadWebsiteMessageCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.dto.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("WebsiteMessage"));

        entity.SetRead();
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteWebsiteMessageCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.WebsiteMessageId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("WebsiteMessage"));

        await _repository.RemoveAsync(entity);
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
        var query = await _repository.GetQueryableAsync()!;
        var condition = await CreateFilteredPredicate(inputDto);
        return query.Where(condition);
    }

    [EventHandler]
    public async Task SyncClickHouseAsync(WebsiteMessageSyncCKCommand command)
    {
        var args = new WebsiteMessageSyncCKJobArgs()
        {
            Time = command.Time,
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }

    [EventHandler]
    public async Task TagsSyncClickHouseAsync(WebsiteMessageTagSyncCKCommand command)
    {
        var args = new WebsiteMessageTagSyncCKJobArgs()
        {
            Time = command.Time,
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}