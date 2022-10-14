// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class UserService : ServiceBase
{
    public UserService(IServiceCollection services) : base("api/user")
    {

    }

    public async Task<UserDto?> CreateExternalUserAsync(IEventBus eventBus, [FromBody] CreateExternalUserDto inputDto)
    {
        var command = new CreateExternalUserCommand(inputDto);
        await eventBus.PublishAsync(command);
        return command.Result;
    }
}
