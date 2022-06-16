// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverSelect : AdminCompontentBase
{
    [Parameter]
    public List<ReceiverGroupItemDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<ReceiverGroupItemDto>> ValueChanged { get; set; }

    private ExternalUserCreateModal _createModal = default!;
    private SubjectAutoComplete _subjectRef = default!;
    private List<Guid> _userIds = new List<Guid>();

    public async Task AddAsync()
    {
        var dtos = _subjectRef.SubjectSelect.Select(x => new ReceiverGroupItemDto
        {
            Type = (ReceiverGroupItemTypes)((int)x.SubjectType),
            SubjectId = x.SubjectId,
            DisplayName = x.Name ?? x.DisplayName ?? string.Empty,
            Avatar = x.Avatar ?? string.Empty,
            PhoneNumber = x.PhoneNumber ?? string.Empty,
            Email = x.Email ?? string.Empty
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

    private async Task HandleOk(UserDto user)
    {
        Value.Add(new ReceiverGroupItemDto
        {
            Type = ReceiverGroupItemTypes.User,
            SubjectId = user.Id,
            DisplayName = user.Name ?? user.DisplayName ?? string.Empty,
            Avatar = user.Avatar,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty
        });
        await ValueChanged.InvokeAsync(Value);
        await SuccessMessageAsync(T("ExternalMemberAddMessage"));
    }

    private void HandleSubjectSelected()
    {
        Console.WriteLine("1");
    }
}
