namespace Masa.Mc.Web.Admin.Components.Modules.MessageTasks;
public partial class MessageTaskReceiverAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<MessageTaskReceiverDto> OnSelectedItemUpdate { get; set; }

    public List<MessageTaskReceiverDto> Items { get; set; } = new();

    public List<MessageTaskReceiverDto> SubjectSelect { get; set; } = new();

    public string Search { get; set; } = "";

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        if (Search == "")
        {
            Items.Clear();
        }
        else if (Search == search)
        {
            var subjectList = await MessageTaskService.GetMessageTaskReceiverListAsync(search);
            Items = subjectList;
        }
    }

    public string TextView(MessageTaskReceiverDto subject)
    {
        if (!string.IsNullOrEmpty(subject.DisplayName)) return subject.DisplayName;
        if (!string.IsNullOrEmpty(subject.PhoneNumber)) return subject.PhoneNumber;
        if (!string.IsNullOrEmpty(subject.Email)) return subject.Email;
        return "";
    }

    private async Task HandleValueChanged(List<Guid> value)
    {
        value = value ?? new();
        var list = Items.Where(x => value.Contains(x.SubjectId)).ToList();
        SubjectSelect = list;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
    }
}

