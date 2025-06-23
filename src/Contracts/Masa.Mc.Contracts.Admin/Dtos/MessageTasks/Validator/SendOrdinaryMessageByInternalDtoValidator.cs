// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendOrdinaryMessageByInternalDtoValidator : AbstractValidator<SendOrdinaryMessageByInternalInputDto>
{
    public SendOrdinaryMessageByInternalDtoValidator()
    {
        RuleFor(x => x.ChannelType).Required();
        RuleFor(x => x.ChannelCode).Required();
        RuleFor(x => x.ReceiverType).IsInEnum();
        RuleFor(x => x.Receivers).ForEach(x => x.SetValidator(new InternalReceiverDtoValidator())).Required().When(x => x.ReceiverType == ReceiverTypes.Assign);
        RuleFor(x => x.SendRules).SetValidator(new SendRuleDtoValidator());
        RuleFor(x => x.MessageInfo).SetValidator(new MessageInfoUpsertDtoValidator());
    }
}