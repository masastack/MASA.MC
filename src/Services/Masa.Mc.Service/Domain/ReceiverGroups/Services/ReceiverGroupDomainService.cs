// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Services;

public class ReceiverGroupDomainService : DomainService
{
    private readonly IReceiverGroupRepository _repository;

    public ReceiverGroupDomainService(IDomainEventBus eventBus, IReceiverGroupRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public virtual async Task CreateAsync(ReceiverGroup receiverGroup, List<ReceiverGroupItem> items = null)
    {
        if (items != null)
        {
            SetItems(receiverGroup, items);
        }
        await _repository.AddAsync(receiverGroup);
        if (items != null)
        {
            await EventBus.PublishAsync(new ReceiverGroupItemChangedDomainEvent(receiverGroup.Id));
        }
    }

    public virtual async Task UpdateAsync(ReceiverGroup receiverGroup, List<ReceiverGroupItem> items = null)
    {
        if (items != null)
        {
            SetItems(receiverGroup, items);
        }
        await _repository.UpdateAsync(receiverGroup);
        if (items != null)
        {
            await EventBus.PublishAsync(new ReceiverGroupItemChangedDomainEvent(receiverGroup.Id));
        }
    }

    public virtual void SetUsers(ReceiverGroup receiverGroup, params Guid[] userIds)
    {
        foreach (var userId in receiverGroup.Users.Select(x => x.UserId).ToArray())
        {
            if (!userIds.Contains(userId))
            {
                receiverGroup.RemoveUser(userId);
            }
        }
        foreach (var userId in userIds)
        {
            if (!receiverGroup.IsInUser(userId))
            {
                receiverGroup.AddUser(userId);
            }
        }
    }

    public virtual void SetItems(ReceiverGroup receiverGroup, List<ReceiverGroupItem> items)
    {
        foreach (var item in items)
        {
            receiverGroup.AddOrUpdateItem(item.SubjectId, item.Type, item.DisplayName, item.Avatar, item.PhoneNumber, item.Email);
        }
        receiverGroup.Items.RemoveAll(item => !items.Any(x => x.SubjectId == item.SubjectId && x.Type == item.Type));
    }
}
