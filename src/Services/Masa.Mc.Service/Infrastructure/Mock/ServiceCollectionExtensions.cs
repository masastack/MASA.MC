// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Mock;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMockService(this IServiceCollection services)
    {
        var smsSenderMock = new Mock<ISmsSender>();

        smsSenderMock.Setup(x => x.SendAsync(It.IsAny<SmsMessage>())).ReturnsAsync(new SmsSendResponse(true, "ok", string.Empty, null));
        smsSenderMock.Setup(x => x.SendBatchAsync(It.IsAny<BatchSmsMessage>())).ReturnsAsync(new BatchSmsSendResponse(true, "ok", string.Empty, null));
        services.Replace(ServiceDescriptor.Singleton<ISmsSender>(smsSenderMock.Object));

        return services;
    }
}