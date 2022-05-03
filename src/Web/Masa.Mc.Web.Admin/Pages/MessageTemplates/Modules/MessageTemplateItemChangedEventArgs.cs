// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTemplates.Modules;

public class MessageTemplateItemChangedEventArgs
{
    public string OldCode { get; set; } = string.Empty;

    public string NewCode { get; set; } = string.Empty;

    public MessageTemplateItemChangedEventArgs(string oldCode, string newCode)
    {
        OldCode = oldCode;
        NewCode = newCode;
    }
}
