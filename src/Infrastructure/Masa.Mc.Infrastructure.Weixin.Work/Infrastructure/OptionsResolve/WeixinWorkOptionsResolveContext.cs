// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve;

public class WeixinWorkOptionsResolveContext
{
    public IWeixinWorkOptions Options { get; set; }

    public IServiceProvider ServiceProvider { get; }

    public WeixinWorkOptionsResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
