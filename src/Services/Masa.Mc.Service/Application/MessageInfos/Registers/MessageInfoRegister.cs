// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageInfos.Registers;

public class MessageInfoRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<MessageInfoUpsertDto, MessageInfo>().MapToConstructor(true)
            .Map(dest => dest.MessageContent, src => new MessageContent(src.Title, src.Content, src.Markdown, src.IsJump, src.JumpUrl));
    }
}