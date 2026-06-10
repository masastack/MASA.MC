// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Constants;

public static class MessageTemplateExceptionCodes
{
    public const string VERIFICATION_CODE_TEMPLATE_CANNOT_ENABLE_UNSUBSCRIBE = "VerificationCodeTemplateCannotEnableUnsubscribe";
    public const string UNSUBSCRIBE_KEYWORD_REQUIRED = "UnsubscribeKeywordRequired";
    public const string RESUBSCRIBE_KEYWORD_REQUIRED = "ResubscribeKeywordRequired";
    public const string UNSUBSCRIBE_KEYWORDS_MUST_BE_DIFFERENT = "UnsubscribeKeywordsMustBeDifferent";
    public const string UNSUBSCRIBE_COOLDOWN_SECONDS_REQUIRED = "UnsubscribeCooldownSecondsRequired";
}
