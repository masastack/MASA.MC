// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverSelect : AdminCompontentBase
{
    [Parameter]
    public List<ReceiverGroupItemDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<ReceiverGroupItemDto>> ValueChanged { get; set; }

    private ExternalUserCreateModal _createModal;
    private SubjectAutoComplete _subjectRef;
    private List<Guid> _userIds = new List<Guid>();
    private List<SubjectDataDto> _items = new();
    private bool _loading;

    SubjectService SubjectService => McCaller.SubjectService;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _items = SubjectDataService.GetList();
    }

    public void Remove(SubjectDataDto item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }

    public async Task AddAsync()
    {
        var dtos = _subjectRef.Items.Select(x => new ReceiverGroupItemDto
        {
            Type = (ReceiverGroupItemTypes)((int)x.SubjectType),
            SubjectId = x.SubjectId,
            DisplayName = x.Name ?? x.DisplayName,
            Avatar = x.Avatar,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email
        });

        foreach (var dto in dtos)
        {
            if (!Value.Any(x => x.SubjectId == dto.SubjectId && x.Type == dto.Type))
            {
                Value.Insert(0, dto);
            }
        }
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task RemoveValue(ReceiverGroupItemDto item)
    {
        Value.RemoveAll(x => x.SubjectId == item.SubjectId && x.Type == item.Type);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleOk(SubjectDataDto user)
    {
        _items.Add(user);
        Value.Add(user.Adapt<ReceiverGroupItemDto>());
        await ValueChanged.InvokeAsync(Value);
        await SuccessMessageAsync(T("ExternalMemberAddMessage"));
    }

    private bool CustomFilter(SubjectDataDto item, string queryText, string text)
    {
        return item.DisplayName.Contains(queryText) ||
          item.PhoneNumber.Contains(queryText) ||
          item.Email.Contains(queryText);
    }
}
