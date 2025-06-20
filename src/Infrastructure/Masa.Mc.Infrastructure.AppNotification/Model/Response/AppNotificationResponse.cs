// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Model.Response;

public class AppNotificationResponse
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public string MsgId { get; set; } = string.Empty;

    public string RegId { get; set; } = string.Empty;

    public AppNotificationResponse(bool success, string message, string msgId = "", string regId = "")
    {
        Success = success;
        Message = message;
        MsgId = msgId;
        RegId = regId;
    }
}
