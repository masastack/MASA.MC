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

            config.ForType<SendTemplateMessageByExternalInputDto, SendSimpleMessageArgs>()
                .Map(dest => dest.ChannelUserIdentity, src => src.Receivers.First().ChannelUserIdentity)
                .Map(dest => dest.Variables, src => ConvertToVariables(src));
        }

        private ExtraPropertyDictionary ConvertToVariables(SendTemplateMessageByExternalInputDto dto)
        {
            if (dto.Receivers.Any())
            {
                var receiver = dto.Receivers.First();
                if (receiver.Variables != null && receiver.Variables.Any())
                {
                    return receiver.Variables;
                }
            }

            return dto.Variables ?? new ExtraPropertyDictionary();
        }
    }
}
