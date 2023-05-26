﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Registers
{
    public class MessageTaskRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTask, MessageTaskDto>().MapToConstructor(true)
                .Map(dest => dest.Receivers, src => src.Receivers);
            config.ForType<MessageTaskUpsertDto, MessageTask>().MapToConstructor(true).Ignore(x=>x.SystemId);
            config.ForType<MessageTaskHistory, MessageTaskHistoryDto>().MapToConstructor(true);
            config.ForType<MessageTaskReceiverDto, MessageTaskReceiver>().MapToConstructor(true)
                 .Map(dest => dest.Receiver, src => new Receiver(src.SubjectId, src.DisplayName, src.Avatar, src.PhoneNumber, src.Email));
            config.ForType<MessageTaskReceiver, MessageTaskReceiverDto>().MapToConstructor(true)
                 .Map(dest => dest.SubjectId, src => src.Receiver.SubjectId)
                 .Map(dest => dest.DisplayName, src => src.Receiver.DisplayName)
                 .Map(dest => dest.Avatar, src => src.Receiver.Avatar)
                 .Map(dest => dest.PhoneNumber, src => src.Receiver.PhoneNumber)
                 .Map(dest => dest.Email, src => src.Receiver.Email);
            config.ForType<ChannelTypes?, ChannelType?>().ConstructUsing(x => x.HasValue ? Enumeration.FromValue<ChannelType>((int)x.Value) : null);
        }
    }
}