// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Model.Response;

public class SmsResponseBase
{
    public bool Success { get; }

    public string Message { get; }

    public string MsgId { get; }

    public SmsResponseBase(bool success, string message, string msgId)
    {
        Success = success;
        Message = message;
        MsgId = msgId;
    }
}
