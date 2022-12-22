// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;


public class SendRuleDtoValidator : AbstractValidator<SendRuleDto>
{
    public SendRuleDtoValidator()
    {
        RuleFor(dto => dto.CronExpression).Required().WithMessage("CronExpressionRequired")
            .Must(x => CronExpression.IsValidExpression(x)).WithMessage("CronExpressionInvalid").When(x => x.IsCustom);
    }
}