// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class ExternalUserCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback<UserDto> OnOk { get; set; }

    private CreateExternalUserDto _model = new();
    private bool _visible;

    UserService UserService => McCaller.UserService;

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
        var user = await UserService.CreateExternalUserAsync(_model);

        _visible = false;

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync(user);
        }

        ResetForm();
    }

    private void ResetForm()
    {
        _model = new();
    }
}