// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Model.Response;

public class AppNotificationResponseBase
{
    public bool Success { get; }

    public string Message { get; }

    public string MsgId { get; set; } = string.Empty;

    public AppNotificationResponseBase(bool success, string message, string msgId = "")
    {
        Success = success;
        Message = message;
        MsgId = msgId;
    }
}
