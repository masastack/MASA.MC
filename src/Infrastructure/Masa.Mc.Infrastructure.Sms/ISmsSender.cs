// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public interface ISmsSender
{
    bool SupportsTemplate { get; }

    bool SupportsReceipt { get; }

    Task<SmsResponseBase> SendAsync(SmsMessage smsMessage);

    Task<SmsResponseBase> SendBatchAsync(BatchSmsMessage smsMessage);
}
