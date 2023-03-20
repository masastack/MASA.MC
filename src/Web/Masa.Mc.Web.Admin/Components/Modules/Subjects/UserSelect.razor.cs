// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class UserSelect : AdminCompontentBase
{
    IAutoCompleteClient? _autocompleteClient;

    [Parameter]
    public Guid? Value { get; set; }

    [Parameter]
    public EventCallback<Guid?> ValueChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string Label { get; set; } = default!;

    [Parameter]
    public int Page { get; set; } = 1;

    [Parameter]
    public int PageSize { get; set; } = 10;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [Parameter]
    public EventCallback<UserSelectModel> OnSelectedItemUpdate { get; set; }

    protected List<UserSelectModel> Items = new();

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
            var response = await AutoCompleteClient.GetBySpecifyDocumentAsync<UserSelectModel>(search, new AutoCompleteOptions
            {
                Page = Page,
                PageSize = PageSize,
            });
            Items = response.Data;
        }
    }

    public string TextView(UserSelectModel user)
    {
        if (!string.IsNullOrEmpty(user.DisplayName)) return user.DisplayName;
        if (!string.IsNullOrEmpty(user.Name)) return user.Name;
        if (!string.IsNullOrEmpty(user.Account)) return user.Account;
        if (!string.IsNullOrEmpty(user.PhoneNumber)) return user.PhoneNumber;
        if (!string.IsNullOrEmpty(user.Email)) return user.Email;
        return "";
    }
}
