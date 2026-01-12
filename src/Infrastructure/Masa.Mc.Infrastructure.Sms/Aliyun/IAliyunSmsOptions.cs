// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public interface IAliyunSmsOptions : IOptions
{
    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string EndPoint { get; set; }

    /// <summary>
    /// MNS队列名称，用于接收短信回执。格式：Alicom-Queue-******-SmsReport
    /// </summary>
    public string? MnsQueueName { get; set; }

    /// <summary>
    /// 是否启用MNS队列消费模式（默认false，使用HTTP回调模式）
    /// </summary>
    public bool EnableMnsConsumer { get; set; }
}