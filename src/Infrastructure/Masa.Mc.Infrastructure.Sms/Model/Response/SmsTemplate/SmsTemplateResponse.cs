// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Model.Response.SmsTemplate;

public class SmsTemplateResponse : SmsResponseBase
{
    public QuerySmsTemplateResponse Data { get; set; }

    public SmsTemplateResponse(bool success, string message, QuerySmsTemplateResponse data) : base(success, message, string.Empty)
    {
        Data = data;
    }
}