// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class SmsTemplateSynchroInput
{
    public Guid ChannelId { get; set; }

    public SmsTemplateSynchroInput(Guid channelId)
    {
        ChannelId = channelId;
    }
}
