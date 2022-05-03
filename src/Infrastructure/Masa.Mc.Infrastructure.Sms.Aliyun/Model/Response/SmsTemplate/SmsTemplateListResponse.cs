// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SmsTemplate;

public class SmsTemplateListResponse : SmsResponseBase
{
    public QuerySmsTemplateListResponse Data { get; set; }

    public SmsTemplateListResponse(bool success, string message, QuerySmsTemplateListResponse data) : base(success, message)
    {
        Data = data;
    }
}
