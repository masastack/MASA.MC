// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageRecords.Events;

public record RetryWeixinMiniProgramMessageEvent : RetryMessageEvent
{
    public RetryWeixinMiniProgramMessageEvent(Guid messageRecordId)
        : base(messageRecordId)
    {

    }
}
