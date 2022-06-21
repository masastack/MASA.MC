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
    private List<Guid> _userIds = new List<Guid>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _userIds = Value.Select(x=>x.SubjectId).ToList();
    }

    public async Task RemoveValue(ReceiverGroupItemDto item)
    {
        Value.RemoveAll(x => x.SubjectId == item.SubjectId);
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

    private async Task HandleSubjectSelected(SubjectDto item)
    {
        if (Value.Any(x=>x.SubjectId== item.SubjectId))
        {
            Value.RemoveAll(x => x.SubjectId == item.SubjectId);
        }
        else
        {
            Value.Add(new ReceiverGroupItemDto
            {
                Type = (ReceiverGroupItemTypes)((int)item.SubjectType),
                SubjectId = item.SubjectId,
                DisplayName = item.Name ?? item.DisplayName ?? string.Empty,
                Avatar = item.Avatar ?? string.Empty,
                PhoneNumber = item.PhoneNumber ?? string.Empty,
                Email = item.Email ?? string.Empty
            });
        }
        await ValueChanged.InvokeAsync(Value);
    }

    public void ResetForm()
    {
        _userIds = new();
    }
}
