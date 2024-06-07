// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateUpsertDtoValidator : AbstractValidator<MessageTemplateUpsertDto>
{
    public MessageTemplateUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required("MessageTemplateDisplayNameRequired")
            .Length(2, 50).WithMessage("MessageTemplateDisplayNameLength");
        RuleFor(inputDto => inputDto.Code).Required("MessageTemplateCodeRequired")
            .LetterNumberSymbol().WithMessage("MessageTemplateCodeLetterNumberSymbol")
            .Length(2, 50).WithMessage("MessageTemplateCodeLength");
        RuleFor(inputDto => inputDto.ChannelId).Required("ChannelIdRequired");
        RuleFor(inputDto => inputDto.Status).IsInEnum().WithMessage("MessageTemplateStatusRequired");
        RuleFor(inputDto => inputDto.AuditStatus).IsInEnum();
        RuleFor(inputDto => inputDto.Sign).Required("SignRequired")
            .Length(2, 12).WithMessage("SignLength")
            .ChineseLetterNumber().WithMessage("SignChineseLetterNumber").When(x => x.ChannelType == ChannelTypes.Sms);
        RuleFor(inputDto => inputDto.PerDayLimit).InclusiveBetween(0, 500).WithMessage("MessageTemplatePerDayLimitBetween");
        RuleFor(inputDto => inputDto.TemplateId).Required("MessageTemplateTemplateIdRequired").When(x => x.ChannelType == ChannelTypes.Sms);
        RuleFor(inputDto => inputDto.Title).Required("TitleRequired")
            .Length(2, 50).WithMessage("TitleLength")
            .When(x => x.ChannelType == ChannelTypes.Email || x.ChannelType == ChannelTypes.WebsiteMessage);
        RuleFor(inputDto => inputDto.Content).Required("ContentRequired");
        RuleFor(inputDto => inputDto.JumpUrl).Required("JumpUrlRequired").When(x => x.IsJump);
        RuleFor(inputDto => inputDto.Items).Must(x => !x.GroupBy(y => y.Code).Any(z => z.Count() > 1)).WithMessage("MessageTemplateItemsCannotRepeated");
    }
}