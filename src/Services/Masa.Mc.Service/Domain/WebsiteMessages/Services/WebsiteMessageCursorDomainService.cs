// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;

public class WebsiteMessageCursorDomainService : DomainService
{
    private readonly IWebsiteMessageCursorRepository _repository;

    public WebsiteMessageCursorDomainService(IDomainEventBus eventBus, IWebsiteMessageCursorRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public virtual async Task CheckAsync(Guid userId)
    {
        var cursor = await _repository.FindAsync(x => x.UserId == userId);
        if (cursor == null)
        {
            cursor = new WebsiteMessageCursor(userId, DateTime.UtcNow);
            await _repository.AddAsync(cursor);
        }
        else
        {
            await EventBus.PublishAsync(new WebsiteMessageCreatedDomainEvent(userId, cursor.UpdateTime));
            cursor.Update();
            await _repository.UpdateAsync(cursor);
        }
    }
}
