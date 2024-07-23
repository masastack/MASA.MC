// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;

public class MessageInfoUpsertDtoValidator : AbstractValidator<MessageInfoUpsertDto>
{
    public MessageInfoUpsertDtoValidator()
    {
        RuleFor(dto => dto.Content).Required("ContentRequired");
        RuleFor(dto => dto.JumpUrl).Required("JumpUrlRequired").When(m => m.IsJump);
    }
}
