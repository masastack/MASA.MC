// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

//In order to support external users, the user ID of each channel is added
public class MessageReceiverUser : Entity<Guid>
{
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();
}
