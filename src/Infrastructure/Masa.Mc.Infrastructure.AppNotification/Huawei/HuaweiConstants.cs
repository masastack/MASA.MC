// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei
{
    public static class HuaweiConstants
    {
        /// <summary>
        /// OAuth 2.0 URL to obtain Access Token
        /// </summary>
        public const string OAuthTokenUrl = "https://oauth-login.cloud.huawei.com/oauth2/v3/token";

        /// <summary>
        /// HMS Push message sending base URL (replace {projectId})
        /// </summary>
        public const string PushMessageUrlFormat = "https://push-api.cloud.huawei.com/v2/{0}/messages:send";

        public const string TopicSubscribeUrlFormat = "https://push-api.cloud.huawei.com/v1/{0}/topic:subscribe";

        public const string TopicUnsubscribeUrlFormat = "https://push-api.cloud.huawei.com/v1/{0}/topic:unsubscribe";
    }
}