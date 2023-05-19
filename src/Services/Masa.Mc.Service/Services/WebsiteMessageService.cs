// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class WebsiteMessageService : ServiceBase
{
    public WebsiteMessageService(IServiceCollection services) : base("api/website-message")
    {
        MapGet(GetListAsync, string.Empty);
        MapGet(GetChannelListAsync);
        MapGet(GetNoticeListAsync);
    }

    public async Task<PaginatedListDto<WebsiteMessageDto>> GetListAsync(IEventBus eventbus, [FromQuery] WebsiteMessageFilterType? filterType, [FromQuery] Guid? channelId, [FromQuery] bool? isRead, [FromQuery] string tag = "", [FromQuery] string channelCode = "", [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetWebsiteMessageInputDto(filter, filterType, channelId, channelCode, isRead, tag, sorting, page, pagesize);
        var query = new GetWebsiteMessageListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<WebsiteMessageDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetWebsiteMessageQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<WebsiteMessageChannelDto>> GetChannelListAsync(IEventBus eventbus)
    {
        var query = new GetChannelListWebsiteMessageQuery();
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task SetAllReadAsync(IEventBus eventbus, [FromBody] GetWebsiteMessageInputDto inputDto)
    {
        var command = new ReadAllWebsiteMessageCommand(inputDto);
        await eventbus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteWebsiteMessageCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task ReadAsync(IEventBus eventbus, [FromBody] ReadWebsiteMessageInputDto inputDto)
    {
        var command = new ReadWebsiteMessageCommand(inputDto);
        await eventbus.PublishAsync(command);
    }

    public async Task CheckAsync(IEventBus eventbus)
    {
        var command = new CheckWebsiteMessageCursorCommand();
        await eventbus.PublishAsync(command);
    }

    public async Task<List<WebsiteMessageDto>> GetNoticeListAsync(IEventBus eventbus, [FromQuery] int pageSize = 5)
    {
        var query = new GetNoticeListQuery(pageSize);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task SendCheckNotificationAsync(IEventBus eventbus)
    {
        var command = new SendCheckNotificationCommand();
        await eventbus.PublishAsync(command);
    }

    public async Task SendGetNotificationAsync(IEventBus eventbus, List<string> userIds)
    {
        var command = new SendGetNotificationCommand(userIds);
        await eventbus.PublishAsync(command);
    }

    public async Task<List<WebsiteMessageDto>> GetListByTagAsync(IEventBus eventbus, string tags, string channelCode)
    {
        var query = new GetListByTagQuery(tags, channelCode);
        await eventbus.PublishAsync(query);
        return query.Result;
    }
}