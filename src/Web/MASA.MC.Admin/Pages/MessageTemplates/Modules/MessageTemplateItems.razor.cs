namespace MASA.MC.Admin.Pages.MessageTemplates.Modules;

public partial class MessageTemplateItems : AdminCompontentBase
{
    private List<DataTableHeader<MessageTemplateItemDto>> _headers = new List<DataTableHeader<MessageTemplateItemDto>>
        {
          new (){Text= "Code",Value= nameof(MessageTemplateItemDto.Code),Sortable=false},
          new (){ Text= "MappingCode", Value= nameof(MessageTemplateItemDto.MappingCode),Sortable=false},
          new (){ Text= "DisplayText", Value= nameof(MessageTemplateItemDto.DisplayText),Sortable=false},
          new (){ Text= "Actions", Value= "actions",Sortable=false }
        };

    private List<MessageTemplateItemDto> _desserts;

    private bool _dialog;
    private bool _dialogDelete;
    private MessageTemplateItemDto _editedItem = new MessageTemplateItemDto();
    private int _editedIndex;

    public string FormTitle
    {
        get
        {
            return _editedIndex == -1 ? "New Item" : "Edit Item";
        }
    }

    protected override void OnInitialized()
    {
        Initialize();
    }

    public void Initialize()
    {
        _desserts = new List<MessageTemplateItemDto>
        {
           new MessageTemplateItemDto
           {
              Code= "name",
              MappingCode= "field1",
              DisplayText= "名称",
            },
            new MessageTemplateItemDto
            {
              Code= "place",
              MappingCode= "field2",
              DisplayText= "地点",
            }
        };
    }

    public void Close()
    {
        _dialog = false;
        _editedItem = new MessageTemplateItemDto();
        _editedIndex = -1;
    }

    public void Save()
    {
        if (_editedIndex > -1)
        {
            var item = _desserts[_editedIndex];
            item.Code = _editedItem.Code;
            item.MappingCode = _editedItem.MappingCode;
            item.DisplayText = _editedItem.DisplayText;
        }
        else
        {
            _desserts.Add(_editedItem);
        }
        Close();
    }

    public void EditItem(MessageTemplateItemDto item)
    {
        _editedIndex = _desserts.IndexOf(item);
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
        _editedIndex = _desserts.IndexOf(item);
        _editedItem = new MessageTemplateItemDto()
        {
            Code = item.Code,
            MappingCode = item.MappingCode,
            DisplayText = item.DisplayText
        };
        _dialogDelete = true;
    }

    public void DeleteItemConfirm()
    {
        _desserts.RemoveAt(_editedIndex);
        CloseDelete();
    }

    public void CloseDelete()
    {
        _dialogDelete = false;
        _editedItem = new();
        _editedIndex = -1;
    }
}
