// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Mock;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMockService(this IServiceCollection services)
    {
        var smsSenderMock = new Mock<ISmsSender>();
        smsSenderMock.Setup(x => x.SendAsync(It.IsAny<SmsMessage>())).ReturnsAsync(new SmsResponseBase(true, "ok", string.Empty));
        smsSenderMock.Setup(x => x.SendBatchAsync(It.IsAny<BatchSmsMessage>())).ReturnsAsync(new SmsResponseBase(true, "ok", string.Empty));
        services.Replace(ServiceDescriptor.Singleton<ISmsSender>(smsSenderMock.Object));

        services.Replace(ServiceDescriptor.Singleton<AliyunSmsSender>(sp =>
            new AliyunSmsSenderFake(
                sp.GetRequiredService<IOptionsResolver<IAliyunSmsOptions>>(),
                sp.GetRequiredService<ILogger<AliyunSmsSender>>()
            )
        ));

        services.Replace(ServiceDescriptor.Singleton<YunMasSmsSender>(sp =>
            new YunMasSmsSenderFake(
                sp.GetRequiredService<IOptionsResolver<IYunMasOptions>>(),
                new HttpClient(),
                sp.GetRequiredService<ILogger<YunMasSmsSender>>()
            )
        ));

        return services;
    }
}