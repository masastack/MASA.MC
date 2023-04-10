// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class ExternalUserCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback<UserDto> OnOk { get; set; }

    [Inject]
    public IAuthClient AuthClient { get; set; } = default!;

    private CreateExternalUserDto _model = new();
    private bool _visible;

    private MForm _form;

    public async Task OpenModalAsync()
    {
        ResetForm();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        _form?.Reset();
    }

    private async Task HandleOk()
    {
        if (!_form.Validate())
        {
            return;
        }

        var user = await CreateExternalUserAsync(_model);

        _visible = false;
        _form?.Reset();

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync(user);
        }
    }

    private void ResetForm()
    {
        _model = new();
    }

    private async Task<UserDto> CreateExternalUserAsync(CreateExternalUserDto dto)
    {
        var requestData = new AddUserModel
        {
            Account = dto.PhoneNumber ?? dto.Email,
            Name = dto.DisplayName,
            DisplayName = dto.DisplayName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email
        };
        var user = await AuthClient.UserService.AddAsync(requestData);
        return user.Adapt<UserDto>(); ;
    }
}