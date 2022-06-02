// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Subjects;

public class UserCommandHandler
{
    readonly IAuthClient _authClient;

    public UserCommandHandler(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    [EventHandler]
    public async Task CreateExternalUserAsync(CreateExternalUserCommand command)
    {
        var createExternalUserDto = command.ExternalUser;
        var requestData = new AddUserModel(createExternalUserDto.PhoneNumber ?? createExternalUserDto.Email, createExternalUserDto.DisplayName)
        {
            DisplayName = createExternalUserDto.DisplayName,
            PhoneNumber = createExternalUserDto.PhoneNumber,
            Email = createExternalUserDto.Email
        };
        var user = await _authClient.UserService.AddAsync(requestData);
        command.Result = user;
    }
}
