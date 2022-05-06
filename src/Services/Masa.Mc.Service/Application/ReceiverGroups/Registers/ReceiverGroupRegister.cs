// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Registers;

public class ReceiverGroupRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<ReceiverGroup, ReceiverGroupDto>().MapToConstructor(true);
        config.ForType<ReceiverGroupUserDto, ReceiverGroupUser>().MapToConstructor(true);
        config.ForType<ReceiverGroupUpsertDto, ReceiverGroup>().MapToConstructor(true).Ignore(x => x.Items);
        config.ForType<ReceiverGroupItemDto, ReceiverGroupItem>().MapToConstructor(true);
    }
}
