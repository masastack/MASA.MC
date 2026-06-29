// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTemplates.Aggregates;

public class MessageTemplateUnsubscribeConfig : ValueObject
{
    // EF Core owned type FK (split table key), keep CLR property to avoid shadow key tracking issues.
    public Guid MessageTemplateId { get; protected set; }

    public bool Enabled { get; protected set; }

    public string UnsubscribeKeyword { get; protected set; } = string.Empty;

    public string UnsubscribeAutoReplyTemplateId { get; protected set; } = string.Empty;

    public string ResubscribeKeyword { get; protected set; } = string.Empty;

    public string ResubscribeAutoReplyTemplateId { get; protected set; } = string.Empty;

    public bool DebounceEnabled { get; protected set; }

    public int CooldownSeconds { get; protected set; }

    private MessageTemplateUnsubscribeConfig()
    {
    }

    public MessageTemplateUnsubscribeConfig(
        bool enabled,
        string unsubscribeKeyword,
        string unsubscribeAutoReplyTemplateId,
        string resubscribeKeyword,
        string resubscribeAutoReplyTemplateId,
        bool debounceEnabled,
        int cooldownSeconds)
    {
        Enabled = enabled;
        UnsubscribeKeyword = (unsubscribeKeyword ?? string.Empty).Trim();
        UnsubscribeAutoReplyTemplateId = (unsubscribeAutoReplyTemplateId ?? string.Empty).Trim();
        ResubscribeKeyword = (resubscribeKeyword ?? string.Empty).Trim();
        ResubscribeAutoReplyTemplateId = (resubscribeAutoReplyTemplateId ?? string.Empty).Trim();
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

    public SmsInboundKeywordAction ResolveInboundKeywordAction(string inboundKeyword)
    {
        if (!Enabled || string.IsNullOrWhiteSpace(inboundKeyword))
        {
            return SmsInboundKeywordAction.None;
        }

        var normalizedKeyword = inboundKeyword.Trim();
        if (string.Equals(normalizedKeyword, UnsubscribeKeyword, StringComparison.OrdinalIgnoreCase))
        {
            return SmsInboundKeywordAction.Unsubscribe;
        }

        if (string.Equals(normalizedKeyword, ResubscribeKeyword, StringComparison.OrdinalIgnoreCase))
        {
            return SmsInboundKeywordAction.Resubscribe;
        }

        return SmsInboundKeywordAction.None;
    }

    public string GetAutoReplyTemplateId(SmsInboundKeywordAction action)
    {
        return action switch
        {
            SmsInboundKeywordAction.Unsubscribe => UnsubscribeAutoReplyTemplateId,
            SmsInboundKeywordAction.Resubscribe => ResubscribeAutoReplyTemplateId,
            _ => string.Empty
        };
    }

    public void Apply(MessageTemplateUnsubscribeConfig config)
    {
        Check.NotNull(config, nameof(config));

        Enabled = config.Enabled;
        UnsubscribeKeyword = config.UnsubscribeKeyword;
        UnsubscribeAutoReplyTemplateId = config.UnsubscribeAutoReplyTemplateId;
        ResubscribeKeyword = config.ResubscribeKeyword;
        ResubscribeAutoReplyTemplateId = config.ResubscribeAutoReplyTemplateId;
        DebounceEnabled = config.DebounceEnabled;
        CooldownSeconds = config.CooldownSeconds;

        NormalizeAndValidate();
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Enabled;
        yield return UnsubscribeKeyword;
        yield return UnsubscribeAutoReplyTemplateId;
        yield return ResubscribeKeyword;
        yield return ResubscribeAutoReplyTemplateId;
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
            UnsubscribeAutoReplyTemplateId = string.Empty;
            ResubscribeKeyword = string.Empty;
            ResubscribeAutoReplyTemplateId = string.Empty;
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

        if (!DebounceEnabled)
        {
            CooldownSeconds = 0;
        }
    }
}
