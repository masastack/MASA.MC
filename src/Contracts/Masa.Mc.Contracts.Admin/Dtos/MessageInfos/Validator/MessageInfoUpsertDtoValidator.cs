// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageInfos.Validator;

public class MessageInfoUpsertDtoValidator : AbstractValidator<MessageInfoUpsertDto>
{
    public MessageInfoUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.Title).Required();
        RuleFor(inputDto => inputDto.Content).Required();
    }
}
