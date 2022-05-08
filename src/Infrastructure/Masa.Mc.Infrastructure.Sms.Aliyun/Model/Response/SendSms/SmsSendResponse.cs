// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SendSms;

public class SmsSendResponse : SmsResponseBase
{
    public SendSmsResponse Data { get; set; }

    public SmsSendResponse(bool success, string message, SendSmsResponse data) : base(success, message)
    {
        Data = data;
    }
}