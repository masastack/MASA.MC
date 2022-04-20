namespace Masa.Mc.Web.Admin.Pages.ReceiverGroups.Modules;

public partial class ReceiverGroupEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    [Inject]
    public ReceiverGroupCaller ReceiverGroupCaller { get; set; } = default!;

    private ReceiverGroupCreateUpdateDto _model = new();
    private Guid _entityId;
    private bool _visible;

    public async Task OpenModalAsync(ReceiverGroupDto model)
    {
        _entityId = model.Id;
        _model = model.Adapt<ReceiverGroupCreateUpdateDto>();
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await ReceiverGroupCaller.GetAsync(_entityId);
        _model = dto.Adapt<ReceiverGroupCreateUpdateDto>();
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
        await ReceiverGroupCaller.UpdateAsync(_entityId, _model);
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
        await ConfirmAsync(T("DeletionConfirmationMessage"),async args =>
        {
            await DeleteAsync();
        }
        );
    }
    private async Task DeleteAsync()
    {
        Loading = true;
        await ReceiverGroupCaller.DeleteAsync(_entityId);
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
    }

    private void HandleVisibleChanged(bool val)
    {
        if (!val) HandleCancel();
    }
}