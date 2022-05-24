// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;


public class SendRuleDtoValidator : AbstractValidator<SendRuleDto>
{
    public SendRuleDtoValidator()
    {
        RuleFor(dto => dto.SendingInterval).Required().When(x => x.IsSendingInterval);
        RuleFor(dto => dto.SendingCount).Required().When(x => x.IsSendingInterval);
        RuleFor(dto => dto.SendTime).Required().When(x => x.IsTiming);
    }
}