// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Registers
{
    public class ChannelRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Channel, ChannelDto>().MapToConstructor(true);
            config.ForType<ChannelUpsertDto, Channel>().MapToConstructor(true)
                .Map(dest => dest.Type, src => Enumeration.FromValue<ChannelType>((int)src.Type));
        }
    }
}