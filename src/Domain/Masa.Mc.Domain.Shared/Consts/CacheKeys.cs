// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Shared.Consts;

public class CacheKeys
{
    public const string MESSAGE_CURSOR_CHECK_COUNT = nameof(MESSAGE_CURSOR_CHECK_COUNT);
    public const string GET_NOTICE_LIST = nameof(GET_NOTICE_LIST);
    public const string CLIENT_CREDENTIALS_TOKEN = "client_credentials_token:";

    public static string ClientCredentialsTokenKey(string clientId)
    {
        return $"{CLIENT_CREDENTIALS_TOKEN}{clientId}";
    }
}