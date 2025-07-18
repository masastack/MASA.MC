// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Services;

public class WebsiteMessageCursorDomainService : DomainService
{
    private readonly IWebsiteMessageCursorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public WebsiteMessageCursorDomainService(IDomainEventBus eventBus
        , IWebsiteMessageCursorRepository repository
        , IUnitOfWork unitOfWork) : base(eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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
            var eventData = new AddWebsiteMessageDomainEvent(userId, cursor.UpdateTime);
            await EventBus.PublishAsync(eventData);
            cursor.Update();
            await _repository.UpdateAsync(cursor);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
    }
}
