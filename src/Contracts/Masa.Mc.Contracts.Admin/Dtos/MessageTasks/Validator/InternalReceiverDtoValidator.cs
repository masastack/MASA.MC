// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class InternalReceiverDtoValidator : AbstractValidator<InternalReceiverDto>
{
    public InternalReceiverDtoValidator()
    {
        RuleFor(x => x.SubjectId).Required();
        RuleFor(x => x.Type).IsInEnum();
    }
}
