// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Auth.Subjects.Dto;

public class CreateExternalUserDtoValidator : AbstractValidator<CreateExternalUserDto>
{
    public CreateExternalUserDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required().Length(2, 50).ChineseLetterNumber();
        RuleFor(inputDto => inputDto.PhoneNumber).Required().Phone().When(u => string.IsNullOrEmpty(u.Email));
        RuleFor(inputDto => inputDto.Email).Required().Email().When(u => string.IsNullOrEmpty(u.PhoneNumber));
    }
}