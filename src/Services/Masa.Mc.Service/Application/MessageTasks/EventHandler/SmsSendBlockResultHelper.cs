// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

internal static class SmsSendBlockResultHelper
{
    public static string GetUnsubscriptionBlockedMessage(II18n<DefaultResource> i18n)
    {
        return i18n.T("MessageBlockedByUnsubscription");
    }

    public static string GetDailySendingLimitMessage(II18n<DefaultResource> i18n)
    {
        return i18n.T("DailySendingLimit");
    }
}
