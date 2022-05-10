// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class SmsTemplateSyncInputDto
{
    public Guid ChannelId { get; set; }

    public SmsTemplateSyncInputDto(Guid channelId)
    {
        ChannelId = channelId;
    }
}
