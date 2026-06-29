// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public record SmsInboundAutoReplySendRequest(
    Guid ChannelId,
    SmsInboundProviders Provider,
    string ChannelUserIdentity,
    Guid UserId,
    Guid AutoReplyTemplateEntityId,
    string AutoReplyTemplateId,
    string AutoReplyTemplateName,
    string AutoReplyContent);
