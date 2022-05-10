// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks.Validator;

public class MessageTaskUpsertDtoValidator : AbstractValidator<MessageTaskUpsertDto>
{
    public MessageTaskUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.ChannelId).Required();
        RuleFor(inputDto => inputDto.EntityId).Required().When(x => x.EntityType == MessageEntityTypes.Template).When(x => !x.IsDraft);
        RuleFor(inputDto => inputDto.EntityType).IsInEnum();
        RuleFor(inputDto => inputDto.ReceiverType).IsInEnum();
        RuleFor(inputDto => inputDto.Receivers).Required().When(x => x.ReceiverType == ReceiverTypes.Assign).When(x => !x.IsDraft);
        RuleFor(inputDto => inputDto.MessageInfo).SetValidator(new MessageInfoUpsertDtoValidator()).When(x => x.EntityType == MessageEntityTypes.Ordinary);
    }
}
