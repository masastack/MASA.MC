// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class SendTestMessageModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private SendTestMessageTaskInputDto _input = new();
    private bool _visible;
    private ChannelTypes? _type;
    private List<Guid> _userIds = new List<Guid>();
    private Components.Modules.Subjects.UserAutoComplete _userRef;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    public async Task OpenModalAsync(Guid messageTaskId, ChannelTypes? type)
    {
        _input.Id = messageTaskId;
        _type = type;
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
    {
        Loading = true;
        await MessageTaskService.SendTestAsync(_input);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskSendMessage"));
        _visible = false;

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
        ResetForm();
    }

    private void ResetForm()
    {
        _input = new();
    }

    private void HandleUserChange()
    {
        _input.Receivers = _userRef.UserSelect.Select(x => new MessageTaskReceiverDto
        {
            SubjectId = x.Id,
            DisplayName = x.Name,
            Avatar = x.Avatar,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            Type = MessageTaskReceiverTypes.User
        }).ToList();
    }
}
