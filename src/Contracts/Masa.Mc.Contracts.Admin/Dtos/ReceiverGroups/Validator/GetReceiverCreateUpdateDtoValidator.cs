// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups.Validator;

public class ReceiverGroupCreateUpdateDtoValidator : AbstractValidator<ReceiverGroupCreateUpdateDto>
{
    public ReceiverGroupCreateUpdateDtoValidator()
    {
        RuleFor(input => input.DisplayName).Required().Length(2, 50).ChineseLetterNumber();
    }
}
