// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceiversSelect : AdminCompontentBase
{
    //[Parameter]
    //public List<MessageTaskReceiverDto> Value { get; set; } = new();

    //[Parameter]
    //public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public ChannelTypes? Type { get; set; }

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> OnAdd { get; set; }

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

    public async Task AddAsync()
    {
        var list = _items.Where(x => _userIds.Contains(x.Id)).ToList();
        var dtos = list.Adapt<List<MessageTaskReceiverDto>>();
        if (OnAdd.HasDelegate)
        {
            await OnAdd.InvokeAsync(dtos);
        }
        //foreach (var dto in dtos)
        //{
        //    if (!Value.Any(x => x.SubjectId == dto.SubjectId && x.Type == dto.Type))
        //    {
        //        Value.Insert(0, dto);
        //    }
        //}
        //await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleOk(SubjectDto user)
    {
        _items.Add(user);
        var dtos = new List<MessageTaskReceiverDto> { user.Adapt<MessageTaskReceiverDto>() };
        if (OnAdd.HasDelegate)
        {
            await OnAdd.InvokeAsync(dtos);
        }
        //Value.Add(user.Adapt<MessageTaskReceiverDto>());
        //await ValueChanged.InvokeAsync(Value);
    }

    public bool CustomFilter(SubjectDto item, string queryText, string text)
    {
        return item.DisplayName.Contains(queryText) ||
          item.PhoneNumber.Contains(queryText) ||
          item.Email.Contains(queryText);
    }
}
