// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Notifications;

public class SendNotificationDto
{
    public string MethodName { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public List<string> UserIds { get; set; } = new();
}
