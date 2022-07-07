// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class OssService : ServiceBase
{
    public OssService(IServiceCollection services) : base(services, "api/oss")
    {
        MapGet(GetSecurityTokenAsync);
    }

    private async Task<GetSecurityTokenDto> GetSecurityTokenAsync([FromServices] IClient client, [FromServices] DaprClient daprClient)
    {
        var region = "oss-cn-hangzhou";
        var response = client.GetSecurityToken();
        var stsToken = response.SessionToken;
        var accessId = response.AccessKeyId;
        var accessSecret = response.AccessKeySecret;
        var bucket = (await daprClient.GetSecretAsync("localsecretstore", "ali-masa-cdn-dev"))["bucket"];
        return await Task.FromResult(new GetSecurityTokenDto(region, accessId, accessSecret, stsToken, bucket));
    }
}