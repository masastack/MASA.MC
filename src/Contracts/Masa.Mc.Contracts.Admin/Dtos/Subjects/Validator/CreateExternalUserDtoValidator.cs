// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Subjects.Validator;

public class CreateExternalUserDtoValidator : AbstractValidator<CreateExternalUserDto>
{
    public CreateExternalUserDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required().Length(2, 50).ChineseLetterNumber();
        RuleFor(inputDto => inputDto.PhoneNumber).Required().Phone().When(u => string.IsNullOrEmpty(u.Email) || !string.IsNullOrEmpty(u.PhoneNumber));
        RuleFor(inputDto => inputDto.Email).Required()
            .Email().WithMessage("EmailInvalid")
            .When(u => string.IsNullOrEmpty(u.PhoneNumber) || !string.IsNullOrEmpty(u.Email));
    }
}