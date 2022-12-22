// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;

public class MessageInfoUpsertDtoValidator : AbstractValidator<MessageInfoUpsertDto>
{
    public MessageInfoUpsertDtoValidator()
    {
        RuleFor(dto => dto.Title).Required().WithMessage("TitleRequired").Length(2, 50);
        RuleFor(dto => dto.Content).Required().WithMessage("ContentRequired");
        RuleFor(dto => dto.JumpUrl).Required().WithMessage("JumpUrlRequired").When(m => m.IsJump);
    }
}
