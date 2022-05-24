// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public partial class MessageTemplateItems : AdminCompontentBase
{
    [Parameter]
    public List<MessageTemplateItemDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTemplateItemDto>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<MessageTemplateItemChangedEventArgs> OnEditChanged { get; set; }

    private List<DataTableHeader<MessageTemplateItemDto>> _headers = new();
    private bool _dialog;
    private bool _dialogDelete;
    private MessageTemplateItemDto _editedItem = new MessageTemplateItemDto();
    private int _editedIndex = -1;

    public string FormTitle
    {
        get
        {
            return _editedIndex == -1 ? T("Permission.AddMessageTemplateItem") : T("Permission.EditMessageTemplateItem");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var _prefix = "DisplayName.MessageTemplateItem";
        _headers = new List<DataTableHeader<MessageTemplateItemDto>>
        {
          new (){ Text= T($"{_prefix}{nameof(MessageTemplateItemDto.Code)}"),Value= nameof(MessageTemplateItemDto.Code),Sortable=false},
          new (){ Text= T($"{_prefix}{nameof(MessageTemplateItemDto.MappingCode)}"), Value= nameof(MessageTemplateItemDto.MappingCode),Sortable=false},
          new (){ Text= T($"{_prefix}{nameof(MessageTemplateItemDto.DisplayText)}"), Value= nameof(MessageTemplateItemDto.DisplayText),Sortable=false},
          new (){ Text= T("Action"), Value= "actions",Sortable=false }
        };
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
            if (item.Code != _editedItem.Code && OnEditChanged.HasDelegate)
            {
                await OnEditChanged.InvokeAsync(new MessageTemplateItemChangedEventArgs(item.Code, _editedItem.Code));
            }
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
