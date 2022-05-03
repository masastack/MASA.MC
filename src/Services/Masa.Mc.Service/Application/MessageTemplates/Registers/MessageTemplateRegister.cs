// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Registers;

public class MessageTemplateRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MessageTemplate, MessageTemplateDto>().MapToConstructor(true);
        config.ForType<MessageTemplateItemDto, MessageTemplateItem>().MapToConstructor(true);
        config.ForType<MessageTemplateCreateUpdateDto, MessageTemplate>().MapToConstructor(true).Ignore(x => x.Items);
        config.ForType<SmsTemplate, SmsTemplateDto>().MapToConstructor(true);
    }
}
