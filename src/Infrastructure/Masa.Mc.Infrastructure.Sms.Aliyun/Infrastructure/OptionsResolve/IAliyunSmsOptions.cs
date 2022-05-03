// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public interface IAliyunSmsOptions
{
    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string EndPoint { get; set; }
}