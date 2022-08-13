// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskUpsertDtoValidator : AbstractValidator<MessageTaskUpsertDto>
{
    public MessageTaskUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.ChannelType).Required().When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.ChannelId).Required().When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.EntityId).Required().When(m => m.EntityType == MessageEntityTypes.Template).When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.EntityType).IsInEnum().When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.ReceiverType).IsInEnum().When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.ReceiverType).Must(m => m == ReceiverTypes.Assign).When(m => m.ChannelType != ChannelTypes.WebsiteMessage);
        RuleFor(inputDto => inputDto.SelectReceiverType).IsInEnum().When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.Receivers).Required().When(m => m.ReceiverType == ReceiverTypes.Assign && !m.IsDraft);
        RuleFor(inputDto => inputDto.MessageInfo).SetValidator(new MessageInfoUpsertDtoValidator()).When(m => m.EntityType == MessageEntityTypes.Ordinary).When(m => !m.IsDraft);
        RuleFor(inputDto => inputDto.SendRules).SetValidator(new SendRuleDtoValidator());
    }
}
