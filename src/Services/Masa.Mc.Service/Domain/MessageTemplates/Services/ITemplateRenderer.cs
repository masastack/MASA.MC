// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public interface ITemplateRenderer
{
    string Render(string context, ExtraPropertyDictionary model, string startstr = "{{", string endstr = "}}");
}
