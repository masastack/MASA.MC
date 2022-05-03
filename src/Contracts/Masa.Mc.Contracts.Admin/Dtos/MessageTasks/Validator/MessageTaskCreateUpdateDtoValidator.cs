﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskCreateUpdateDtoValidator : AbstractValidator<MessageTaskCreateUpdateDto>
{
    public MessageTaskCreateUpdateDtoValidator()
    {
        RuleFor(input => input.ChannelId).Required();
        RuleFor(input => input.EntityId).Required().When(x => x.EntityType == MessageEntityType.Template);
        RuleFor(input => input.EntityType).IsInEnum();
        RuleFor(input => input.ReceiverType).IsInEnum();
        RuleFor(input => input.Receivers).Must(x => x.Items.Any()).When(x => x.ReceiverType == ReceiverType.Assign);
        RuleFor(input => input.MessageInfo).SetValidator(new MessageInfoCreateUpdateDtoValidator()).When(x => x.EntityType == MessageEntityType.Ordinary);
    }
}
