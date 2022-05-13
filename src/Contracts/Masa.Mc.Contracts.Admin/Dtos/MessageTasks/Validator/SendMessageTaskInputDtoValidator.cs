// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class SendMessageTaskInputDtoValidator : AbstractValidator<SendMessageTaskInputDto>
{
    public SendMessageTaskInputDtoValidator()
    {
        RuleFor(inputDto => inputDto.ReceiverType).IsInEnum();
        RuleFor(inputDto => inputDto.Receivers).Required().When(x => x.ReceiverType == ReceiverTypes.Assign);
        RuleFor(inputDto => inputDto.Sign).Required().ChineseLetterNumber().Length(2, 12).When(m => m.ChannelType == ChannelTypes.Sms);
    }
}