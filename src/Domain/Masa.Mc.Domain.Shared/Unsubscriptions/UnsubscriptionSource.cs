// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Shared.Unsubscriptions;

public enum UnsubscriptionSource
{
    SmsInboundKeyword = 1,
    ManualSupport,
    AppSetting,
    H5Page,
    SystemAutomation
}
