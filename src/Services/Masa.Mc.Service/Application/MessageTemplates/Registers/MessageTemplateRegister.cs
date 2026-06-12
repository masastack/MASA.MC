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
        config.ForType<MessageTemplateItemDto, MessageTemplateItem>().MapToConstructor(true);
        config.ForType<MessageTemplateUpsertDto, MessageTemplate>()
            .MapToConstructor(false)
            .ConstructUsing(src => new MessageTemplate(
                src.ChannelId,
                src.DisplayName,
                src.Code,
                new MessageContent(src.Title, src.Content, src.Markdown, src.IsJump, src.JumpUrl, src.ExtraProperties),
                src.Example,
                src.TemplateId,
                src.Sign,
                src.TemplateType,
                src.PerDayLimit,
                src.UnsubscribeConfig.Adapt<MessageTemplateUnsubscribeConfig>(),
                src.Status,
                src.AuditStatus,
                src.AuditReason,
                src.IsStatic))
            .Ignore(x => x.Items)
            .Map(dest => dest.MessageContent, src => new MessageContent(src.Title, src.Content, src.Markdown, src.IsJump, src.JumpUrl, src.ExtraProperties))
            .Map(dest => dest.Options, src => src.Options ?? new());
        config.ForType<MessageTemplate, MessageTemplateDto>().MapToConstructor(true)
            .Map(dest => dest.UnsubscribeConfig, src => src.GetUnsubscribeConfig().Adapt<MessageTemplateUnsubscribeConfigDto>());
        config.ForType<SmsTemplate, SmsTemplateDto>().MapToConstructor(true);
    }
}
