// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public static class OppoConstants
{
    public const string AuthUrl = "https://api.push.oppomobile.com/server/v1/auth";
    public const string UnicastUrl = "https://api.push.oppomobile.com/server/v1/message/notification/unicast";
    public const string UnicastBatchUrl = "https://api.push.oppomobile.com/server/v1/message/notification/unicast_batch";
    public const string SaveMessageContentUrl = "https://api.push.oppomobile.com/server/v1/message/notification/save_message_content";
    public const string BroadcastUrl = "https://api.push.oppomobile.com/server/v1/message/notification/broadcast";
    public const string SubscribeTagsUrl = "https://api-device.push.heytapmobi.com/server/v1/device/subscribe_tags";
    public const string UnsubscribeTagsUrl = "https://api-device.push.heytapmobi.com/server/v1/device/unsubscribe_tags";
}
