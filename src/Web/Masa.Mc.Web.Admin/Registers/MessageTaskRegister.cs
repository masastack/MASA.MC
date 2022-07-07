// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Registers;

public class MessageTaskRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MessageTaskDto, MessageTaskUpsertDto>().MapToConstructor(true)
            .Map(dest => dest.ChannelType, src => src.Channel.Type);
    }
}