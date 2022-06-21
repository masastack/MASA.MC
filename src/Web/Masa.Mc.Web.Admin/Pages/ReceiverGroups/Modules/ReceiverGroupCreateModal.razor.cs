// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverGroupCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private ReceiverGroupUpsertDto _model = new();
    private MForm _form = default!;
    private ReceiverSelect _ReceiverSelect = default!;
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

    private async Task HandleCancel()
    {
        _visible = false;
        await ResetForm();
    }

    private async Task HandleOk(EditContext context)
    {
        if (!context.Validate())
        {
            return;
        }
        Loading = true;
        await ReceiverGroupService.CreateAsync(_model);
        Loading = false;
        await SuccessMessageAsync(T("ReceiverGroupCreateMessage"));
        _visible = false;
        await ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task ResetForm()
    {
        _model = new();
        await _form.ResetValidationAsync();
        _ReceiverSelect.ResetForm();
    }

    private async Task HandleVisibleChanged(bool val)
    {
        if (!val) await HandleCancel();
    }
}
