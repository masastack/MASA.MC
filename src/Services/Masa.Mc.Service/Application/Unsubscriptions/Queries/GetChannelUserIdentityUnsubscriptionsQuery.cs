// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions.Queries;

public record GetChannelUserIdentityUnsubscriptionsQuery(string ChannelCode, string ChannelUserIdentity) : Query<List<ChannelUserIdentityUnsubscriptionItemDto>>
{
    public override List<ChannelUserIdentityUnsubscriptionItemDto> Result { get; set; } = new();
}
