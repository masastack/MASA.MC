// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.Oss;

public class OssService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public OssService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/oss/";
    }

    public async Task<GetSecurityTokenDto> GetSecurityTokenAsync()
    {
        return await GetAsync<GetSecurityTokenDto>(nameof(GetSecurityTokenAsync));
    }
}
