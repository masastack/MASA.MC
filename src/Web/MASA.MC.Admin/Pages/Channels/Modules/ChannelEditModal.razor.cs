namespace MASA.MC.Admin.Pages.Channels.Modules;

public partial class ChannelEditModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private ChannelCreateUpdateDto _model = new();
    private MForm _form;
    private Guid _entityId;
    private bool _visible;
    private List<ChannelType> channelTypeItems = Enum.GetValues(typeof(ChannelType))
        .Cast<ChannelType>().ToList();

    [Inject]
    public ChannelCaller ChannelCaller { get; set; } = default!;

    public async Task OpenModalAsync(ChannelDto model)
    {
        _entityId = model.Id;
        _model = MapToCreateUpdateDto(model);
        await GetFormDataAsync();
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private async Task GetFormDataAsync()
    {
        var dto = await ChannelCaller.GetAsync(_entityId);
        _model = MapToCreateUpdateDto(dto);
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOk(EditContext context)
    {
        if (!await _form.ValidateAsync())
        {
            return;
        }
        await ChannelCaller.UpdateAsync(_entityId, _model);
        _visible = false;
        ResetForm();
        await SuccessMessageAsync(T("Edit channel data success"));
        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
    }

    private async Task HandleDel()
    {
        await ConfirmAsync(T("Are you sure delete data?"),async args =>
        {
            await DeleteAsync();
        }
        );
    }
    private async Task DeleteAsync()
    {
        await ChannelCaller.DeleteAsync(_entityId);
        await SuccessMessageAsync(T("Delete channel data success"));
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

    private ChannelCreateUpdateDto MapToCreateUpdateDto(ChannelDto dto)
    {
        return new ChannelCreateUpdateDto
        {
            DisplayName = dto.DisplayName,
            Code = dto.Code,
            Type = dto.Type,
            Description = dto.Description,
            IsStatic = dto.IsStatic,
            ExtraProperties = dto.ExtraProperties
        };
    }
}
