// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class AppNotificationSenderFactory
{
    private readonly GetuiSender _getuiSender;
    private readonly JPushSender _jPushSender;

    public AppNotificationSenderFactory(GetuiSender getuiSender, JPushSender jPushSender)
    {
        _getuiSender = getuiSender;
        _jPushSender = jPushSender;
    }

    public IAppNotificationSender GetAppNotificationSender(Providers provider)
    {
        switch (provider)
        {
            case Providers.GeTui:
                return _getuiSender;
            case Providers.JPush:
                return _jPushSender;
            default:
                throw new Exception($"Unknow Provider: {provider}");
        }
    }
}
