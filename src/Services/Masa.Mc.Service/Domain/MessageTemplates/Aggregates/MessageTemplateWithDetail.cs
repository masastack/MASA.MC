// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;

public class MessageTemplateWithDetail
{
    public MessageTemplate MessageTemplate { get; set; }
    public Channel Channel { get; set; }
}
