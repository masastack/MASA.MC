// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Registers;

public class MessageTemplateRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MessageTemplateUnsubscribeConfigQueryModel, MessageTemplateUnsubscribeConfigDto>().MapToConstructor(true);
        config.ForType<MessageTemplateUnsubscribeConfig, MessageTemplateUnsubscribeConfigDto>().MapToConstructor(true);
        config.ForType<MessageTemplateUnsubscribeConfigDto, MessageTemplateUnsubscribeConfig>().MapToConstructor(true);
        config.ForType<MessageTemplate, MessageTemplateDto>().MapToConstructor(true);
        config.ForType<MessageTemplateItemDto, MessageTemplateItem>().MapToConstructor(true);
        config.ForType<MessageTemplateUpsertDto, MessageTemplate>().MapToConstructor(true).Ignore(x => x.Items)
            .Map(dest => dest.MessageContent, src => new MessageContent(src.Title, src.Content, src.Markdown, src.IsJump, src.JumpUrl, src.ExtraProperties))
            .Map(dest => dest.UnsubscribeConfig, src => src.UnsubscribeConfig.Adapt<MessageTemplateUnsubscribeConfig>());
        config.ForType<SmsTemplate, SmsTemplateDto>().MapToConstructor(true);
    }
}
