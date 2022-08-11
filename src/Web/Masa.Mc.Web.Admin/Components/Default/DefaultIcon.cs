// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Default;

public class DefaultIcon : SIcon
{
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Size = 20;

        await base.SetParametersAsync(parameters);
    }
}