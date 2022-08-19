namespace Masa.Mc.Web.Admin.Components.Modules.MessageTasks;
public partial class MessageTaskReceiverAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<MessageTaskReceiverDto> OnRemove { get; set; }

    [Parameter]
    public EventCallback<MessageTaskReceiverDto> OnAdd { get; set; }

    [Parameter]
    public List<Guid> SelectedValues { get; set; } = new();

    public List<MessageTaskReceiverDto> Items { get; set; } = new();

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

    public void ResetForm()
    {
        Items = new();
        ResetValue();
    }

    public void ResetValue()
    {
        Value = new();
        if (ValueChanged.HasDelegate)
        {
            ValueChanged.InvokeAsync(Value);
        }
    }

    public void Remove(MessageTaskReceiverDto item)
    {
        var index = Value.IndexOf(item.SubjectId);
        if (index >= 0)
        {
            Value.RemoveAt(index);
        }

        if (ValueChanged.HasDelegate)
        {
            ValueChanged.InvokeAsync(Value);
        }

        if (OnRemove.HasDelegate)
        {
            OnRemove.InvokeAsync(item);
        }
    }

    public void HandleOnSelectedItemUpdate(MessageTaskReceiverDto item)
    {
        if (SelectedValues.Contains(item.SubjectId))
        {
            Remove(item);
        }
        else
        {
            if (OnAdd.HasDelegate)
            {
                OnAdd.InvokeAsync(item);
            }
        }
    }
}

