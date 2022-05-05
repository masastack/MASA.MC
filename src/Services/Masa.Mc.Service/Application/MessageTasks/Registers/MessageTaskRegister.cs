﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Registers
{
    public class MessageTaskRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<MessageTask, MessageTaskDto>().MapToConstructor(true)
                .Map(dest => dest.Receivers, src => src.Receivers)
                .Map(dest => dest.SendingRules, src => ExtensionPropertyHelper.ExtraPropertyMapToObj<SendingRuleDto>(src.SendingRules));
            config.ForType<MessageTaskCreateUpdateDto, MessageTask>().MapToConstructor(true)
                .Map(dest => dest.SendingRules, src => ExtensionPropertyHelper.ObjMapToExtraProperty(src.SendingRules));
            config.ForType<MessageTaskHistory, MessageTaskHistoryDto>().MapToConstructor(true)
                .Map(dest => dest.SendingRules, src => ExtensionPropertyHelper.ExtraPropertyMapToObj<SendingRuleDto>(src.SendingRules));
        }
    }
}
