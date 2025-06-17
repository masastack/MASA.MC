// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Model.Response;

public class AppNotificationResponse
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public string MsgId { get; set; } = string.Empty;

    public List<string> ErrorTokens { get; set; }

    public AppNotificationResponse(bool success, string message, string msgId = "", List<string> errorTokens = null)
    {
        Success = success;
        Message = message;
        MsgId = msgId;
        ErrorTokens = errorTokens ?? new();
    }
}
