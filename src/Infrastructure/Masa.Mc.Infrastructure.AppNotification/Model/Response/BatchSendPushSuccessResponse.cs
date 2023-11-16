// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Model.Response;

public class BatchSendPushSuccessResponse : AppNotificationResponseBase
{
    public ConcurrentDictionary<string, SendPushModelResponse>? Data { get; set; }

    public BatchSendPushSuccessResponse(bool success, string message, ConcurrentDictionary<string, SendPushModelResponse>? data = null) : base(success, message)
    {
        Data = data;
    }
}


public class SendPushModelResponse
{

    public string MsgId { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}