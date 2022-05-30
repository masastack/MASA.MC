// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates.Validator;

public class MessageTemplateUpsertDtoValidator : AbstractValidator<MessageTemplateUpsertDto>
{
    public MessageTemplateUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required();
        RuleFor(inputDto => inputDto.ChannelId).Required();
        RuleFor(inputDto => inputDto.Status).IsInEnum();
        RuleFor(inputDto => inputDto.AuditStatus).IsInEnum();
        RuleFor(inputDto => inputDto.Sign).Required().Length(2, 12).ChineseLetterNumber().When(x => x.ChannelType == ChannelTypes.Sms);
        RuleFor(inputDto => inputDto.PerDayLimit).InclusiveBetween(1, 500);
        RuleFor(inputDto => inputDto.TemplateId).Required().When(x => x.ChannelType == ChannelTypes.Sms);
        RuleFor(inputDto => inputDto.DisplayName).Required().Length(2, 50).ChineseLetterNumber().When(x => x.ChannelType == ChannelTypes.Email);
        RuleFor(inputDto => inputDto.Title).Required().Length(2, 255).When(x => x.ChannelType == ChannelTypes.Email || x.ChannelType == ChannelTypes.WebsiteMessage);
        RuleFor(inputDto => inputDto.Content).Required();
        RuleFor(inputDto => inputDto.JumpUrl).Required().Url().When(x=>x.IsJump);
    }
}