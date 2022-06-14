// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class UserAutoComplete : AdminCompontentBase
{
    IAutoCompleteClient? _autocompleteClient;

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public int Page { get; set; } = 1;

    [Parameter]
    public int PageSize { get; set; } = 10;

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    public List<UserSelectModel> Items { get; set; } = new();

    public List<UserSelectModel> UserSelect { get; set; } = new();

    public string Search { get; set; } = "";

    [Inject]
    public IAutoCompleteClient AutoCompleteClient
    {
        get => _autocompleteClient ?? throw new Exception("Please inject IAutoCompleteClient");
        set => _autocompleteClient = value;
    }

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        if (Search == "")
        {
            Items.Clear();
        }
        else if (Search == search)
        {
            var response = await AutoCompleteClient.GetAsync<UserSelectModel, Guid>(search, new AutoCompleteOptions
            {
                Page = Page,
                PageSize = PageSize,
            });
            Items = response.Data;
        }
    }

    public string TextView(UserSelectModel user)
    {
        if (!string.IsNullOrEmpty(user.Name)) return user.Name;
        if (!string.IsNullOrEmpty(user.Account)) return user.Account;
        if (!string.IsNullOrEmpty(user.PhoneNumber)) return user.PhoneNumber;
        if (!string.IsNullOrEmpty(user.Email)) return user.Email;
        return "";
    }

    private async Task HandleValueChanged(List<Guid> value)
    {
        value = value ?? new();
        var list = Items.Where(x => value.Contains(x.Id)).ToList();
        UserSelect = list;
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
    }
}
