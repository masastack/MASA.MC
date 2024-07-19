// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Contributors;

public class ConfigurationOptionsResolveContributor : IWeixinWorkMessageOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "Configuration";
    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(WeixinWorkMessageOptionsResolveContext context)
    {
        context.Options = context.ServiceProvider.GetRequiredService<IOptions<WeixinWorkMessageOptions>>().Value;

        return Task.CompletedTask;
    }
}