// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class ExternalUserCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback<UserViewModel> OnOk { get; set; }

    private UserViewModel _model = new();
    private bool _visible;

    public async Task OpenModalAsync()
    {
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

    private async Task HandleOk()
    {
        var id = Guid.NewGuid();
        _model.Id = id;
        _model.SubjectId = string.Empty;
        _model.Type = ReceiverGroupItemTypes.User;
        await SuccessMessageAsync(T("ExternalMemberAddMessage"));
        _visible = false;

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync(_model);
        }
        ResetForm();
    }

    private void ResetForm()
    {
        _model = new();
    }
}
