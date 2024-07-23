// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseWeixinWork(this IApplicationBuilder app, IHostEnvironment environment)
    {
        app.UseSenparcWeixin(environment, null, null,
    register => { },
    (register, weixinSetting) =>
    {
    });
        return app;
    }
}
