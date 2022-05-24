// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class ExternalUserCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback<SubjectDto> OnOk { get; set; }

    private CreateExternalUserDto _model = new();
    private bool _visible;

    ReceiverGroupService ReceiverGroupService => McCaller.ReceiverGroupService;

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
        var subject = new SubjectDto
        {
            Id = id,
            SubjectId = id,
            DisplayName = _model.DisplayName,
            PhoneNumber = _model.PhoneNumber,
            Email = _model.Email,
            Type = MessageTaskReceiverTypes.User
        };
        _visible = false;

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync(subject);
        }
        ResetForm();
    }

    private void ResetForm()
    {
        _model = new();
    }
}