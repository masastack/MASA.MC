// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Default;

public class DefaultSheetDialog : SSheetDialog
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        EnableDomReload = true;
        ContentClass ??= "";
        if (ContentClass.Contains("sheetDialogPadding") is false)
            ContentClass += " sheetDialogPadding";
    }
}
