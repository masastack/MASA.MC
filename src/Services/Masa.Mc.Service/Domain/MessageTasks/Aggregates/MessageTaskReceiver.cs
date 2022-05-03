// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskReceiver
{
    public string DataId { get; protected set; } = string.Empty;

    public string DisplayName { get; protected set; } = string.Empty;

    public string Avatar { get; protected set; } = string.Empty;

    public string PhoneNumber { get; protected set; } = string.Empty;

    public string Email { get; protected set; } = string.Empty;

    public MessageTaskReceiverType Type { get; protected set; }
}
