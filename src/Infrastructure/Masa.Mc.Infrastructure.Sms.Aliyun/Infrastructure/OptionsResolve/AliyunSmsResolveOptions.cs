// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public class AliyunSmsResolveOptions
{
    public List<IAliyunSmsOptionsResolveContributor> Contributors { get; }

    public AliyunSmsResolveOptions()
    {
        Contributors = new List<IAliyunSmsOptionsResolveContributor>();
    }
}