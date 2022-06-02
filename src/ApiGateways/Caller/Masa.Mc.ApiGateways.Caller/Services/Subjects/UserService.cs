// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public UserService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/user";
    }

    public async Task<UserModel?> CreateExternalUserAsync(CreateExternalUserDto inputDto)
    {
        return await PostAsync<CreateExternalUserDto, UserModel?>(nameof(CreateExternalUserAsync), inputDto);
    }
}
