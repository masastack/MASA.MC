// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Subjects.Commands;

public record CreateExternalUserCommand(CreateExternalUserDto ExternalUser) : Command
{
    public UserModel? Result { get; set; } = new();
}
