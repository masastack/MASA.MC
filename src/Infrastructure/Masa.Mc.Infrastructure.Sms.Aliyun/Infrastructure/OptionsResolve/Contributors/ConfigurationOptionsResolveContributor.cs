﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve.Contributors;

public class ConfigurationOptionsResolveContributor : IAliyunSmsOptionsResolveContributor
{
    public const string ContributorName = "Configuration";
    public string Name => ContributorName;

    public Task ResolveAsync(AliyunSmsOptionsResolveContext context)
    {
        context.Options = context.ServiceProvider.GetRequiredService<IOptions<AliyunSmsOptions>>().Value;

        return Task.CompletedTask;
    }
}