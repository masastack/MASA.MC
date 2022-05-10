// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public class AliyunSmsOptionsResolveContext
{
    public IAliyunSmsOptions Options { get; set; }

    public IServiceProvider ServiceProvider { get; }

    public AliyunSmsOptionsResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
