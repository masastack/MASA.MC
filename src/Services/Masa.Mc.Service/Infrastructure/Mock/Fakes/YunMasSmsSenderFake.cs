// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Mock.Fakes;

public class YunMasSmsSenderFake : YunMasSmsSender, ISmsSender
{
    public YunMasSmsSenderFake(IOptionsResolver<IYunMasOptions> optionsResolver, HttpClient httpClient, ILogger<YunMasSmsSender> logger)
        : base(optionsResolver, httpClient, logger)
    {
    }

    Task<SmsResponseBase> ISmsSender.SendAsync(SmsMessage smsMessage)
        => Task.FromResult(new SmsResponseBase(true, "ok", string.Empty));

    Task<SmsResponseBase> ISmsSender.SendBatchAsync(BatchSmsMessage smsMessage)
        => Task.FromResult(new SmsResponseBase(true, "ok", string.Empty));
}


