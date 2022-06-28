// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class SendTestMessageTaskInputDto
{
    public Guid Id { get; set; }
    public List<MessageReceiverUserDto> ReceiverUsers { get; set; } = new();
}
