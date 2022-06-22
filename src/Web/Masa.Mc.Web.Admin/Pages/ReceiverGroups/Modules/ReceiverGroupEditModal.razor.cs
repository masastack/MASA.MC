﻿// Copyright (c) MASA Stack All rights reserved.
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
        var dto = await ReceiverGroupService.GetAsync(_entityId);
        _model = dto.Adapt<ReceiverGroupUpsertDto>();
    }

    private async Task HandleCancel()
    {
        _visible = false;
        await ResetForm();
    }

    private async Task HandleOk()
    {
        if (!await _form.ValidateAsync())
        {
            return;
        }
        Loading = true;
        await ReceiverGroupService.UpdateAsync(_entityId, _model);
        Loading = false;
        _visible = false;
        await ResetForm();
        await SuccessMessageAsync(T("ReceiverGroupEditMessage"));
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDel()
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), DeleteAsync);
    }
    private async Task DeleteAsync()
    {
        Loading = true;
        await ReceiverGroupService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("ReceiverGroupDeleteMessage"));
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

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}