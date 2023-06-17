// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Model;

public class AppTemplateUpsertModel: MessageTemplateUpsertDto
{
    public new AppMessageOptions Options { get; set; } = new();
}
