namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverGroupCreateModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private ReceiverGroupCreateUpdateDto _model = new();
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
        ResetForm();
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private void ResetForm()
    {
        _model = new();
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}
