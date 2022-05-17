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
    private List<SubjectDto> _items = new();
    private bool _loading;
    private GetReceiverGroupInputDto _queryParam = new(99);

    ReceiverGroupService ReceiverGroupService => McCaller.ReceiverGroupService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var subjects = SubjectService.GetList();
        var receiverGroups = (await ReceiverGroupService.GetListAsync(_queryParam)).Result.Select(r => new SubjectDto
        {
            Id = r.Id,
            Type = MessageTaskReceiverTypes.Group,
            SubjectId = r.Id,
            DisplayName = r.DisplayName
        });
        _items = receiverGroups.Concat(subjects).ToList();
    }

    public void Remove(SubjectDto item)
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

    private async Task HandleOk(SubjectDto user)
    {
        _items.Add(user);
        var dtos = new List<MessageTaskReceiverDto> { user.Adapt<MessageTaskReceiverDto>() };
        if (await HandleAddAsync(dtos))
        {
            await SuccessMessageAsync(T("ExternalMemberAddMessage"));
        }
    }

    private bool CustomFilter(SubjectDto item, string queryText, string text)
    {
        return item.DisplayName.Contains(queryText) ||
          item.PhoneNumber.Contains(queryText) ||
          item.Email.Contains(queryText);
    }

    private async Task RemoveValue(MessageTaskReceiverDto item)
    {
        Value.Remove(item);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task<bool> HandleAddAsync(List<MessageTaskReceiverDto> receivers)
    {
        foreach (var receiver in receivers)
        {
            if (Value.Any(x => x.PhoneNumber == receiver.PhoneNumber))
            {
                await WarningAsync("手机号不允许重复");
                return false;
            }
            if (Value.Any(x => x.Email == receiver.Email))
            {
                await WarningAsync("邮箱不允许重复");
                return false;
            }
            Value.Insert(0, receiver);
        }
        await ValueChanged.InvokeAsync(Value);
        return true;
    }
}
