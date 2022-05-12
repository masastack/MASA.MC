// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Modules.Subjects;

public partial class UserSelect : AdminCompontentBase
{
    [Parameter]
    public Guid? Value { get; set; }

    [Parameter]
    public EventCallback<Guid?> ValueChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [Parameter]
    public EventCallback<UserDto> OnSelectedItemUpdate { get; set; }

    private bool _isLoading;

    protected List<UserDto> Items = new();

    public void UpdateSearchInput(string val)
    {
        if (Items.Count > 0) return;
        if (_isLoading) return;
        _isLoading = true;
        var res = UserService.GetList();
        Items = res.Where(x => x.DisplayName.Contains(val) || x.PhoneNumber.Contains(val) || x.Email.Contains(val)).Take(20).ToList();
        _isLoading = false;
    }
}
