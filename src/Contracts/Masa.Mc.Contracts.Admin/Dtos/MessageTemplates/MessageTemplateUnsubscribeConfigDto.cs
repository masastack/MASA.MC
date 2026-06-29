// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateUnsubscribeConfigDto
{
    public bool Enabled { get; set; }

    public string UnsubscribeKeyword { get; set; } = string.Empty;

    public string UnsubscribeAutoReplyTemplateId { get; set; } = string.Empty;

    public string ResubscribeKeyword { get; set; } = string.Empty;

    public string ResubscribeAutoReplyTemplateId { get; set; } = string.Empty;

    public bool DebounceEnabled { get; set; }

    public int CooldownSeconds { get; set; }
}
