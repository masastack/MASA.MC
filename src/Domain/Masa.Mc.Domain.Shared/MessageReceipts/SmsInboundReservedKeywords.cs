// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Shared.MessageReceipts;

public static class SmsInboundReservedKeywords
{
    public const string YunMasUnsubscribeKeyword = "R";

    public static bool IsProviderUnsubscribeKeyword(SmsInboundProviders provider, string inboundKeyword)
    {
        if (string.IsNullOrWhiteSpace(inboundKeyword))
        {
            return false;
        }

        var normalizedKeyword = inboundKeyword.Trim();
        return provider switch
        {
            SmsInboundProviders.YunMas => string.Equals(normalizedKeyword, YunMasUnsubscribeKeyword, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
