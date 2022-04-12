namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class MessageTemplateItems : AdminCompontentBase
{
    private List<DataTableHeader<MessageTemplateItemDto>> _headers = new List<DataTableHeader<MessageTemplateItemDto>>
        {
          new (){ Text= "Code",Value= nameof(MessageTemplateItemDto.Code),Sortable=false},
          new (){ Text= "MappingCode", Value= nameof(MessageTemplateItemDto.MappingCode),Sortable=false},
          new (){ Text= "DisplayText", Value= nameof(MessageTemplateItemDto.DisplayText),Sortable=false},
          new (){ Text= "Actions", Value= "actions",Sortable=false }
        };

    [Parameter]
    public List<MessageTemplateItemDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTemplateItemDto>> ValueChanged { get; set; }

    private bool _dialog;
    private bool _dialogDelete;
    private MessageTemplateItemDto _editedItem = new MessageTemplateItemDto();
    private int _editedIndex = -1;

    public string FormTitle
    {
        get
        {
            return _editedIndex == -1 ? T("Permission:AddMessageTemplateItem") : T("Permission:EditMessageTemplateItem");
        }
    }

    public void Close()
    {
        _dialog = false;
        _editedItem = new MessageTemplateItemDto();
        _editedIndex = -1;
    }

    public async Task SaveAsync()
    {
        if (_editedIndex > -1)
        {
            var item = Value[_editedIndex];
            item.Code = _editedItem.Code;
            item.MappingCode = _editedItem.MappingCode;
            item.DisplayText = _editedItem.DisplayText;
        }
        else
        {
            Value.Add(_editedItem);
        }
        await ValueChanged.InvokeAsync(Value);
        Close();
    }

    public void EditItem(MessageTemplateItemDto item)
    {
        _editedIndex = Value.IndexOf(item);
        _editedItem = new MessageTemplateItemDto()
        {
            Code = item.Code,
            MappingCode = item.MappingCode,
            DisplayText = item.DisplayText
        };
        _dialog = true;
    }

    public void DeleteItem(MessageTemplateItemDto item)
    {
        _editedIndex = Value.IndexOf(item);
        _editedItem = new MessageTemplateItemDto()
        {
            Code = item.Code,
            MappingCode = item.MappingCode,
            DisplayText = item.DisplayText
        };
        _dialogDelete = true;
    }

    public async Task DeleteItemConfirmAsync()
    {
        Value.RemoveAt(_editedIndex);
        await ValueChanged.InvokeAsync(Value);
        CloseDelete();
    }

    public void CloseDelete()
    {
        _dialogDelete = false;
        _editedItem = new();
        _editedIndex = -1;
    }
}
