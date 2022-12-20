// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Getui.Infrastructure.OptionsResolve;

public class GetuiResolveOptions
{
    public List<IAppNotificationOptionsResolveContributor> Contributors { get; }

    public GetuiResolveOptions()
    {
        Contributors = new List<IAppNotificationOptionsResolveContributor>();
    }
}