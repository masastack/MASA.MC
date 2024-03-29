﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class UserAutoComplete : AdminCompontentBase
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    public List<UserSelectModel> Items { get; set; } = new();

    public List<UserSelectModel> UserSelect { get; set; } = new();

    public string Search { get; set; } = "";

    [Inject]
    public IAuthClient AuthClient { get; set; } = default!;

    public async Task OnSearchChanged(string search)
    {
        Search = search;
        if (Search == "")
        {
            Items.Clear();
        }
        else if (Search == search)
        {
            var response = await AuthClient.UserService.SearchAsync(search);
            Items = response;
        }
    }

    public string TextView(UserSelectModel user)
    {
        if (!string.IsNullOrEmpty(user.DisplayName)) return user.DisplayName;
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
