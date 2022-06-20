// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.ButtonGroup;

public partial class BooleanButtonGroup
{
    [Parameter]
    public bool? Value { get; set; }

    [Parameter]
    public EventCallback<bool?> ValueChanged { get; set; }

    [Parameter]
    public List<KeyValuePair<string, bool>> Items { get; set; } = new();

    [Parameter]
    public StyleTypes StyleType { get; set; }

    public List<KeyValuePair<string, bool>> KeyValues { get; set; } = new();

    protected override void OnParametersSet()
    {
        KeyValues = Items.Any() ? Items : GetBooleanMap();
    }
}