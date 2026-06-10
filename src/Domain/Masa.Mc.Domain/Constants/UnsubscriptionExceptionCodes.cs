// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Constants;

public static class UnsubscriptionExceptionCodes
{
    public const string CHANNEL_USER_IDENTITY_REQUIRED = "ChannelUserIdentityRequired";
    public const string INVALID_UNSUBSCRIPTION_SCOPE_REFERENCE = "InvalidUnsubscriptionScopeReference";
    public const string INVALID_UNSUBSCRIPTION_STATUS_TRANSITION = "InvalidUnsubscriptionStatusTransition";
    public const string UNSUBSCRIPTION_REASON_REQUIRED = "UnsubscriptionReasonRequired";
}
