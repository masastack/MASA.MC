// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class MessageReceiverUserDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
