// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class UserService : ServiceBase
{
    public UserService(IServiceCollection services) : base(services, "api/user")
    {
        MapPost(CreateExternalUserAsync);
    }

    public async Task<UserModel?> CreateExternalUserAsync(IEventBus eventBus, [FromBody] CreateExternalUserDto inputDto)
    {
        var command = new CreateExternalUserCommand(inputDto);
        await eventBus.PublishAsync(command);
        return command.Result;
    }
}
