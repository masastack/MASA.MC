// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class ChannelUserIdentityUnsubscriptionItemDto
{
    public string ChannelUserIdentity { get; set; } = string.Empty;

    public Guid? TemplateId { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public string TemplateContent { get; set; } = string.Empty;
}
