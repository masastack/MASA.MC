namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageSendingRules : AdminCompontentBase
{
    [Parameter]
    public SendingRuleDto Value { get; set; } = new();

    [Parameter]
    public EventCallback<SendingRuleDto> ValueChanged { get; set; }

    private bool _datePickersShow;
    private bool _timePickersShow;
    public DateOnly? sendingDate;
    public TimeOnly? sendingTime;

    protected override void OnParametersSet()
    {
        if (Value.SendTime != null)
        {
            sendingDate = DateOnly.FromDateTime(Value.SendTime.Value);
            sendingTime = TimeOnly.FromDateTime(Value.SendTime.Value);
        }
        else
        {
            sendingDate = null;
            sendingTime = null;
        }
    }

    private async void HandleDatePickerInputAsync()
    {
        _datePickersShow = false;
        await FillSendTimeAsync();
    }

    private async void HandleTimePickerChangeAsync()
    {
        await FillSendTimeAsync();
    }

    private async Task FillSendTimeAsync()
    {
        if (sendingDate == null)
        {
            return;
        }
        Value.SendTime = sendingDate.Value.ToDateTime(sendingTime ?? new TimeOnly(0, 0, 0));
        await ValueChanged.InvokeAsync(Value);
    }
}
