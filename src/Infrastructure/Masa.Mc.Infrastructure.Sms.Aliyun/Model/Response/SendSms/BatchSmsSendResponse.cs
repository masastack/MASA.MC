// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SendSms;

public class BatchSmsSendResponse : SmsResponseBase
{
    public SendBatchSmsResponse Data { get; set; }

    public BatchSmsSendResponse(bool success, string message, SendBatchSmsResponse data) : base(success, message)
    {
        Data = data;
    }
}
