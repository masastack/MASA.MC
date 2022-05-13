﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskUpsertDtoValidator : AbstractValidator<MessageTaskUpsertDto>
{
    public MessageTaskUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.ChannelId).Required();
        RuleFor(inputDto => inputDto.EntityId).Required().When(m => m.EntityType == MessageEntityTypes.Template).When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.EntityType).IsInEnum();
        RuleFor(inputDto => inputDto.ReceiverType).IsInEnum();
        RuleFor(inputDto => inputDto.Receivers).Required().When(m => m.ReceiverType == ReceiverTypes.Assign).When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.MessageInfo).SetValidator(new MessageInfoUpsertDtoValidator()).When(m => m.EntityType == MessageEntityTypes.Ordinary);
        RuleFor(inputDto => inputDto.Sign).Required().ChineseLetterNumber().Length(2, 12).When(m => m.ChannelType == ChannelTypes.Sms && !m.IsDraft);
    }
}