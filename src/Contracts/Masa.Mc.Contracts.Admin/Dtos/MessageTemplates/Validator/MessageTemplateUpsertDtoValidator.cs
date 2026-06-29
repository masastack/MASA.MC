// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateUpsertDtoValidator : AbstractValidator<MessageTemplateUpsertDto>
{
    public MessageTemplateUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required("MessageTemplateDisplayNameRequired")
            .Length(2, 100).WithMessage("MessageTemplateDisplayNameLength").When(x => x.ChannelType != ChannelTypes.WeixinWork && x.TemplateType != (int)WeixinWorkTemplateTypes.Text);
        RuleFor(inputDto => inputDto.Code).Required("MessageTemplateCodeRequired")
            .LetterNumberSymbol().WithMessage("MessageTemplateCodeLetterNumberSymbol")
            .Length(2, 50).WithMessage("MessageTemplateCodeLength");
        RuleFor(inputDto => inputDto.ChannelId).Required("ChannelIdRequired");
        RuleFor(inputDto => inputDto.Status).IsInEnum().WithMessage("MessageTemplateStatusRequired");
        RuleFor(inputDto => inputDto.TemplateType)
            .Must(templateType => System.Enum.IsDefined(typeof(SmsTemplateTypes), templateType))
            .When(inputDto => inputDto.ChannelType == ChannelTypes.Sms)
            .WithMessage("MessageTemplateTemplateTypeRequired");
        RuleFor(inputDto => inputDto.AuditStatus).IsInEnum();
        RuleFor(inputDto => inputDto.PerDayLimit).InclusiveBetween(0, 500).WithMessage("MessageTemplatePerDayLimitBetween");
        RuleFor(inputDto => inputDto.Title).Required("TitleRequired")
            .Length(2, 100).WithMessage("TitleLength")
            .When(x => x.ChannelType == ChannelTypes.Email || x.ChannelType == ChannelTypes.WebsiteMessage);
        RuleFor(inputDto => inputDto.Content).Required("ContentRequired");
        RuleFor(inputDto => inputDto.JumpUrl).Required("JumpUrlRequired").When(x => x.IsJump);
        RuleFor(inputDto => inputDto.Items).Must(x => !x.GroupBy(y => y.Code).Any(z => z.Count() > 1)).WithMessage("MessageTemplateItemsCannotRepeated");

        RuleFor(inputDto => inputDto.UnsubscribeConfig).NotNull();
        RuleFor(inputDto => inputDto.UnsubscribeConfig.UnsubscribeKeyword).Required("UnsubscribeKeywordRequired")
            .Length(1, 20).WithMessage("UnsubscribeKeywordLength")
            .When(x => x.UnsubscribeConfig.Enabled);
        RuleFor(inputDto => inputDto.UnsubscribeConfig.UnsubscribeAutoReply).Required("UnsubscribeAutoReplyRequired")
            .Length(1, 500).WithMessage("UnsubscribeAutoReplyLength")
            .When(x => x.UnsubscribeConfig.Enabled);
        RuleFor(inputDto => inputDto.UnsubscribeConfig.ResubscribeKeyword).Required("ResubscribeKeywordRequired")
            .Length(1, 20).WithMessage("ResubscribeKeywordLength")
            .When(x => x.UnsubscribeConfig.Enabled);
        RuleFor(inputDto => inputDto.UnsubscribeConfig.ResubscribeAutoReply).Required("ResubscribeAutoReplyRequired")
            .Length(1, 500).WithMessage("ResubscribeAutoReplyLength")
            .When(x => x.UnsubscribeConfig.Enabled);
        RuleFor(inputDto => inputDto).Must(inputDto =>
            !inputDto.UnsubscribeConfig.Enabled ||
            !string.Equals(inputDto.UnsubscribeConfig.UnsubscribeKeyword?.Trim(), inputDto.UnsubscribeConfig.ResubscribeKeyword?.Trim(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("UnsubscribeKeywordsMustBeDifferent");
        RuleFor(inputDto => inputDto).Must(inputDto =>
            !inputDto.UnsubscribeConfig.Enabled ||
            inputDto.ChannelType != ChannelTypes.Sms ||
            !string.Equals(inputDto.UnsubscribeConfig.UnsubscribeKeyword?.Trim(), SmsInboundReservedKeywords.YunMasUnsubscribeKeyword, StringComparison.OrdinalIgnoreCase))
            .WithMessage("UnsubscribeKeywordCannotUseProviderReservedKeyword");
        RuleFor(inputDto => inputDto).Must(inputDto =>
            !(inputDto.TemplateType == (int)SmsTemplateTypes.VerificationCode && inputDto.UnsubscribeConfig.Enabled))
            .WithMessage("VerificationCodeTemplateCannotEnableUnsubscribe");
    }
}