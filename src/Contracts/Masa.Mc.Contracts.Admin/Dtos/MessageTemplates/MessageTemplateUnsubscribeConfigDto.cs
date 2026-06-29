// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateUnsubscribeConfigDto
{
    public bool Enabled { get; set; }

    public string UnsubscribeKeyword { get; set; } = string.Empty;

    public Guid UnsubscribeAutoReplyTemplateId { get; set; }

    public string ResubscribeKeyword { get; set; } = string.Empty;

    public Guid ResubscribeAutoReplyTemplateId { get; set; }
}
