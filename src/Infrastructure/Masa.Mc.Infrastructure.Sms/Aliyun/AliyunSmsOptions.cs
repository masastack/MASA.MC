// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsOptions : IAliyunSmsOptions
{
    public string AccessKeySecret { get; set; }

    public string AccessKeyId { get; set; }

    public string EndPoint { get; set; } = "dysmsapi.aliyuncs.com";

    public string? MnsQueueName { get; set; }

    public bool EnableMnsConsumer { get; set; }

    public void SetOptions(AliyunSmsOptions options)
    {
        AccessKeySecret = options.AccessKeySecret;
        AccessKeyId = options.AccessKeyId;
        EndPoint = options.EndPoint;
        MnsQueueName = options.MnsQueueName;
        EnableMnsConsumer = options.EnableMnsConsumer;
    }
}
