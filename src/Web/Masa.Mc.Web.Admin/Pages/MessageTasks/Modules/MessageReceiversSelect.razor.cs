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

    private ExternalUserCreateModal _createModal;
    private List<Guid> _userIds = new List<Guid>();
    private List<SubjectDataDto> _items = new();
    private bool _loading;
    private GetReceiverGroupInputDto _queryParam = new(99);

    ReceiverGroupService ReceiverGroupService => McCaller.ReceiverGroupService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var subjects = SubjectDataService.GetList();
        var receiverGroups = (await ReceiverGroupService.GetListAsync(_queryParam)).Result.Select(r => new SubjectDataDto
        {
            Id = r.Id,
            Type = MessageTaskReceiverTypes.Group,
            SubjectId = r.Id,
            DisplayName = r.DisplayName
        });
        _items = receiverGroups.Concat(subjects).ToList();
    }

    public void Remove(SubjectDataDto item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }

    private async Task AddAsync()
    {
        var list = _items.Where(x => _userIds.Contains(x.Id)).ToList();
        var dtos = list.Adapt<List<MessageTaskReceiverDto>>();
        await HandleAddAsync(dtos);
    }

    private async Task HandleOk(SubjectDataDto user)
    {
        _items.Add(user);
        var dtos = new List<MessageTaskReceiverDto> { user.Adapt<MessageTaskReceiverDto>() };
        await HandleAddAsync(dtos);
    }

    private bool CustomFilter(SubjectDataDto item, string queryText, string text)
    {
        return item.DisplayName.Contains(queryText) ||
          item.PhoneNumber.Contains(queryText) ||
          item.Email.Contains(queryText);
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
}
