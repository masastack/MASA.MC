// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Registers;

public class MessageTemplateRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MessageTemplateDto, AppTemplateUpsertModel>()
            .Map(dest => dest.Options, src => ExtensionPropertyHelper.ConvertToType<AppMessageOptions>(src.Options));
    }
}
