// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class OssService : ServiceBase
{
    public OssService(IServiceCollection services) : base("api/oss")
    {
        
    }

    public async Task<SecurityTokenDto> GetSecurityTokenAsync([FromServices] IClient client, [FromServices] IOptions<OssOptions> ossOptions)
    {
        var region = "oss-cn-hangzhou";
        var response = client.GetSecurityToken();
        var stsToken = response.SessionToken;
        var accessId = response.AccessKeyId;
        var accessSecret = response.AccessKeySecret;
        var bucket = ossOptions.Value.Bucket;
        return await Task.FromResult(new SecurityTokenDto(region, accessId, accessSecret, stsToken, bucket));
    }
}