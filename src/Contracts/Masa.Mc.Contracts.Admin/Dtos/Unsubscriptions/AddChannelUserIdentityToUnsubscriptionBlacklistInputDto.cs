// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class AddChannelUserIdentityToUnsubscriptionBlacklistInputDto
{
    public Guid? UserId { get; set; }

    public string ChannelCode { get; set; } = string.Empty;

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public string TemplateCode { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}
