// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateCreateUpdateDtoValidator : AbstractValidator<MessageTemplateCreateUpdateDto>
{
    public MessageTemplateCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).Required();
        RuleFor(input => input.ChannelId).Required();
        RuleFor(input => input.Status).IsInEnum();
        RuleFor(input => input.AuditStatus).IsInEnum();
        RuleFor(input => input.Sign).Required().Length(2, 12).ChineseLetterNumber().When(x => x.ChannelType == ChannelType.Sms);
        RuleFor(input => input.DayLimit).InclusiveBetween(1, 500);
        RuleFor(input => input.TemplateId).Required().When(x => x.ChannelType == ChannelType.Sms);
        RuleFor(input => input.DisplayName).Required().Length(2, 50).ChineseLetterNumber().When(x => x.ChannelType == ChannelType.Email);
        RuleFor(input => input.Title).Required().Length(2, 255).ChineseLetterNumber().When(x => x.ChannelType == ChannelType.Email || x.ChannelType == ChannelType.WebsiteMessage);
        RuleFor(input => input.Content).Required();
        RuleFor(input => input.JumpUrl).Required().When(x=>x.IsJump);
    }
}