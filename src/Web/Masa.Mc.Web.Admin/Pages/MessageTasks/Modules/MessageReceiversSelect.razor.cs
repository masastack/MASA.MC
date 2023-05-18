// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceiversSelect : AdminCompontentBase
{
    [Parameter]
    public List<MessageTaskReceiverDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public ChannelTypes? Type { get; set; }

    private ExternalUserCreateModal _createModal = default!;

    private async Task HandleOk(UserDto user)
    {
        var receiver = new MessageTaskReceiverDto
        {
            SubjectId = user.Id,
            DisplayName = user.GetDisplayName(),
            Avatar = user.Avatar,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Type = MessageTaskReceiverTypes.User
        };
        var dtos = new List<MessageTaskReceiverDto> { receiver };
        await HandleAddAsync(dtos);
    }

    private async Task RemoveValue(MessageTaskReceiverDto item)
    {
        Value.Remove(item);

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task HandleAddAsync(List<MessageTaskReceiverDto> receivers)
    {
        foreach (var receiver in receivers)
        {
            if (!string.IsNullOrEmpty(receiver.PhoneNumber) && Value.Any(x => x.PhoneNumber == receiver.PhoneNumber)) continue;
            if (!string.IsNullOrEmpty(receiver.Email) && Value.Any(x => x.Email == receiver.Email)) continue;
            if (receiver.SubjectId != default && Value.Any(x => x.SubjectId == receiver.SubjectId)) continue;
            Value.Insert(0, receiver);
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task HandleOnAdd(MessageTaskReceiverDto item)
    {
        if (!Value.Any(x => x.SubjectId == item.SubjectId))
        {
            Value.Add(item);
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task HandleOnRemove(MessageTaskReceiverDto item)
    {
        if (Value.Any(x => x.SubjectId == item.SubjectId))
        {
            Value.RemoveAll(x => x.SubjectId == item.SubjectId);
            await ValueChanged.InvokeAsync(Value);
        }
    }
}
