// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverGroupEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private ReceiverGroupUpsertDto _model = new();
    private MForm _form = default!;
    private ReceiverSelect _ReceiverSelect = default!;
    private Guid _entityId;
    private bool _visible;

    ReceiverGroupService ReceiverGroupService => McCaller.ReceiverGroupService;

    public async Task OpenModalAsync(ReceiverGroupDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<ReceiverGroupUpsertDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await ReceiverGroupService.GetAsync(_entityId) ?? new();
        _model = dto.Adapt<ReceiverGroupUpsertDto>();
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk()
    {
        if (!_form.Validate())
        {
            return;
        }
        Loading = true;
        await ReceiverGroupService.UpdateAsync(_entityId, _model);
        Loading = false;
        _visible = false;
        ResetForm();
        await SuccessMessageAsync(T("ReceiverGroupEditMessage"));
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDel()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage",$"{T("ReceiverGroup")}\"{_model.DisplayName}\""), DeleteAsync, AlertTypes.Error);
    }
    private async Task DeleteAsync()
    {
        Loading = true;
        await ReceiverGroupService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("ReceiverGroupDeleteMessage"));
        _visible = false;
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }
    private void ResetForm()
    {
        _model = new();
        _form.ResetValidation();
        _ReceiverSelect.ResetForm();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}