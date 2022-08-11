// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Registers
{
    public class MessageRecordRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageRecord, MessageRecordDto>().MapToConstructor(true)
                .Map(dest => dest.User, src => src.ExtraProperties);
        }
    }
}
