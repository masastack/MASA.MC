// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve;

public class WeixinWorkMessageResolveOptions
{
    public List<IWeixinWorkMessageOptionsResolveContributor> Contributors { get; }

    public WeixinWorkMessageResolveOptions()
    {
        Contributors = new List<IWeixinWorkMessageOptionsResolveContributor>();
    }
}