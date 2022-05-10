// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceivers : AdminCompontentBase
{
    [Parameter]
    public List<MessageTaskReceiverDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public ChannelTypes? Type { get; set; }

    private ExternalUserCreateModal _createModal;
    private List<Guid> _userIds = new List<Guid>();
    private List<SubjectDto> _stateUserItems = SubjectService.GetList();

    public void Remove(SubjectDto item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }

    public async Task AddAsync()
    {
        var list = _stateUserItems.Where(x => _userIds.Contains(x.Id)).ToList();
        var dtos = list.Adapt<List<MessageTaskReceiverDto>>();
        foreach (var dto in dtos)
        {
            if (!Value.Any(x => x.SubjectId == dto.SubjectId && x.Type == dto.Type))
            {
                Value.Insert(0, dto);
            }
        }
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task RemoveValue(MessageTaskReceiverDto item)
    {
        Value.RemoveAll(x => x.SubjectId == item.SubjectId && x.Type == item.Type);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleOk(SubjectDto user)
    {
        _stateUserItems.Add(user);
        Value.Add(user.Adapt<MessageTaskReceiverDto>());
        await ValueChanged.InvokeAsync(Value);
    }
}
