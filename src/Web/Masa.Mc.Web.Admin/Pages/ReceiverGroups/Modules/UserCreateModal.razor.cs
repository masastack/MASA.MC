﻿namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class UserCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback<UserViewModel> OnOk { get; set; }

    [Inject]
    public ReceiverGroupCaller ReceiverGroupCaller { get; set; } = default!;

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
        _model.DataId = id.ToString();
        _model.Type = ReceiverGroupItemType.User;
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
