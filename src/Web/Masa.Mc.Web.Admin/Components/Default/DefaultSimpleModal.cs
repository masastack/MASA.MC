// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Rendering;

namespace Masa.Mc.Web.Admin.Components.Default;

public class DefaultSimpleModal : SSimpleModal
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Value)
        {
            builder.OpenElement(0, "div");
            builder.SetKey(Value);
            base.BuildRenderTree(builder);
            builder.CloseElement();
        }
    }
}
