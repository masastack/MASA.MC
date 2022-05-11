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
    private List<Guid> _userIds = new List<Guid>();
    private List<SubjectDto> _items = new();
    private List<SubjectDto> _stateUserItems = SubjectService.GetList();
    private bool _loading;

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
        var dtos = list.Adapt<List<ReceiverGroupItemDto>>();
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

    private async Task HandleOk(SubjectDto user)
    {
        _items.Add(user);
        Value.Add(user.Adapt<ReceiverGroupItemDto>());
        await ValueChanged.InvokeAsync(Value);
    }

    private void QuerySelections(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
        {
            return;
        }
        _loading = true;
        _items = _stateUserItems.Where(x => x.DisplayName.Contains(v) || x.PhoneNumber.Contains(v) || x.Email.Contains(v)).ToList();
        _loading = false;
        StateHasChanged();
    }

    public bool CustomFilter(SubjectDto item, string queryText, string text)
    {
        return item.DisplayName.Contains(queryText)||
          item.PhoneNumber.Contains(queryText)||
          item.Email.Contains(queryText);
    }
}
