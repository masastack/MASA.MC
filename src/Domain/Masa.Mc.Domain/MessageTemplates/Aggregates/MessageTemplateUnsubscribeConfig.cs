// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTemplates.Aggregates;

public class MessageTemplateUnsubscribeConfig : ValueObject
{
    public bool Enabled { get; protected set; }

    public string UnsubscribeKeyword { get; protected set; } = string.Empty;

    public string UnsubscribeAutoReply { get; protected set; } = string.Empty;

    public string ResubscribeKeyword { get; protected set; } = string.Empty;

    public string ResubscribeAutoReply { get; protected set; } = string.Empty;

    public bool DebounceEnabled { get; protected set; }

    public int CooldownSeconds { get; protected set; }

    private MessageTemplateUnsubscribeConfig()
    {
    }

    public MessageTemplateUnsubscribeConfig(
        bool enabled,
        string unsubscribeKeyword,
        string unsubscribeAutoReply,
        string resubscribeKeyword,
        string resubscribeAutoReply,
        bool debounceEnabled,
        int cooldownSeconds)
    {
        Enabled = enabled;
        UnsubscribeKeyword = (unsubscribeKeyword ?? string.Empty).Trim();
        UnsubscribeAutoReply = (unsubscribeAutoReply ?? string.Empty).Trim();
        ResubscribeKeyword = (resubscribeKeyword ?? string.Empty).Trim();
        ResubscribeAutoReply = (resubscribeAutoReply ?? string.Empty).Trim();
        DebounceEnabled = debounceEnabled;
        CooldownSeconds = cooldownSeconds;

        NormalizeAndValidate();
    }

    public static MessageTemplateUnsubscribeConfig Disabled()
    {
        return new MessageTemplateUnsubscribeConfig(false, string.Empty, string.Empty, string.Empty, string.Empty, false, 0);
    }

    public string BuildSuffix()
    {
        if (!Enabled || string.IsNullOrWhiteSpace(UnsubscribeKeyword))
        {
            return string.Empty;
        }

        return $"【回复{UnsubscribeKeyword}退订】";
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Enabled;
        yield return UnsubscribeKeyword;
        yield return UnsubscribeAutoReply;
        yield return ResubscribeKeyword;
        yield return ResubscribeAutoReply;
        yield return DebounceEnabled;
        yield return CooldownSeconds;
    }

    private void NormalizeAndValidate()
    {
        if (!Enabled)
        {
            DebounceEnabled = false;
            CooldownSeconds = 0;
            UnsubscribeKeyword = string.Empty;
            UnsubscribeAutoReply = string.Empty;
            ResubscribeKeyword = string.Empty;
            ResubscribeAutoReply = string.Empty;
            return;
        }

        if (string.IsNullOrWhiteSpace(UnsubscribeKeyword))
        {
            throw new UserFriendlyException(errorCode: MessageTemplateExceptionCodes.UNSUBSCRIBE_KEYWORD_REQUIRED);
        }

        if (string.IsNullOrWhiteSpace(ResubscribeKeyword))
        {
            throw new UserFriendlyException(errorCode: MessageTemplateExceptionCodes.RESUBSCRIBE_KEYWORD_REQUIRED);
        }

        if (string.Equals(UnsubscribeKeyword, ResubscribeKeyword, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException(errorCode: MessageTemplateExceptionCodes.UNSUBSCRIBE_KEYWORDS_MUST_BE_DIFFERENT);
        }

        if (DebounceEnabled && CooldownSeconds <= 0)
        {
            throw new UserFriendlyException(errorCode: MessageTemplateExceptionCodes.UNSUBSCRIBE_COOLDOWN_SECONDS_REQUIRED);
        }

        if (!DebounceEnabled)
        {
            CooldownSeconds = 0;
        }
    }
}
