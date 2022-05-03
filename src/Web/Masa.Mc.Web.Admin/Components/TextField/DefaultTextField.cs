// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.TextFieldt;

public class DefaultTextField<TValue> : MTextField<TValue>
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        Dense = true;
        HideDetails = "auto";
        Outlined = true;
        await base.SetParametersAsync(parameters);
    }
}