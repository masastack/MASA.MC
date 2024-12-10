// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;

public class WebsiteMessageCursorDomainService : DomainService
{
    private readonly IWebsiteMessageCursorRepository _repository;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public WebsiteMessageCursorDomainService(IDomainEventBus eventBus
        , IWebsiteMessageCursorRepository repository
        , IHubContext<NotificationsHub> hubContext) : base(eventBus)
    {
        _repository = repository;
        _hubContext = hubContext;
    }

    public virtual async Task CheckAsync(Guid userId)
    {
        var cursor = await _repository.FindAsync(x => x.UserId == userId);
        if (cursor == null)
        {
            cursor = new WebsiteMessageCursor(userId, DateTimeOffset.UtcNow);
            await _repository.AddAsync(cursor);
        }
        else
        {
            await EventBus.PublishAsync(new AddWebsiteMessageDomainEvent(userId, cursor.UpdateTime));
            cursor.Update();
            await _repository.UpdateAsync(cursor);
            await _repository.UnitOfWork.SaveChangesAsync();
            await _repository.UnitOfWork.CommitAsync();

            var onlineClients = _hubContext.Clients.Users(userId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
        }
    }
}
