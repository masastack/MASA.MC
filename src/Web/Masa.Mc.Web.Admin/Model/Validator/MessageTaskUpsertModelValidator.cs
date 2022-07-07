// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Model.Validator;

public class MessageTaskUpsertModelValidator : AbstractValidator<MessageTaskUpsertModel>
{
    public MessageTaskUpsertModelValidator()
    {
        RuleFor(model => model.ChannelType).Required().When(m => m.Step == 1 && !m.IsDraft);
        RuleFor(model => model.ChannelId).Required().When(m => m.Step == 1 && !m.IsDraft);
        RuleFor(model => model.EntityId).Required().When(m => m.EntityType == MessageEntityTypes.Template && m.Step == 1 && !m.IsDraft);
        RuleFor(model => model.EntityType).IsInEnum().When(m => m.Step == 1 && !m.IsDraft);
        RuleFor(model => model.MessageInfo).SetValidator(new MessageInfoUpsertDtoValidator()).When(m => m.EntityType == MessageEntityTypes.Ordinary && m.Step == 1 && !m.IsDraft);

        RuleFor(model => model.ReceiverType).IsInEnum().When(m => m.Step == 2 && !m.IsDraft);
        RuleFor(model => model.SelectReceiverType).IsInEnum().When(m => m.Step == 2 && !m.IsDraft);
        RuleFor(model => model.Receivers).Required().When(m => m.ReceiverType == ReceiverTypes.Assign && m.Step == 2 && !m.IsDraft);

        RuleFor(model => model.Sign).Required().ChineseLetterNumber().Length(2, 12).When(m => m.ChannelType == ChannelTypes.Sms && m.Step == 3 && !m.IsDraft);
        RuleFor(model => model.SendRules).SetValidator(new SendRuleDtoValidator()).When(m => m.Step == 3 && !m.IsDraft);
    }
}
