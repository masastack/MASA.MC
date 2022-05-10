// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Selects;

public partial class BooleanSelect
{
    [Parameter]
    public string Placeholder { get; set; } = "";

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public bool? Value { get; set; }

    [Parameter]
    public EventCallback<bool?> ValueChanged { get; set; }

    [Parameter]
    public List<KeyValuePair<string, bool>> Items { get; set; } = new();

    [Parameter]
    public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [Parameter]
    public EventCallback<KeyValuePair<string, bool>> OnSelectedItemUpdate { get; set; }

    public List<KeyValuePair<string, bool>> KeyValues { get; set; } = new();

    protected override void OnParametersSet()
    {
        KeyValues = Items.Any() ? Items : GetBooleanMap();
    }
}

